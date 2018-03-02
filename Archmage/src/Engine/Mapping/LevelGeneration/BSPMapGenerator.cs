using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archmage.GameData;
using Archmage.Engine.DataStructures;

namespace Archmage.Mapping.LevelGeneration
{
    class BSPMapGenerator
    {
        int width = 50;
        int height = 30;
        int minRoomDimensions = 5;
        int numOfSubdivisions = 7;

        Random rand;

        LevelData level;

        public BSPMapGenerator()
        {

        }

        public void GenerateMapFrame(LevelData level)//generates floor surrounded by walls
        {
            width = level.width;
            height = level.height;
            int x = 0;
            int y = 0;

            for (x = 0; x < width; x++)
            {
                level.tiles[x, y] = new TileData(x, y, "wall");//first row gen

            }

            for (y = 1; y < height - 1; y++)
            {
                level.tiles[0, y] = new TileData(x, y, "wall");//side walls
                for (x = 1; x < width - 1; x++)
                {
                    level.tiles[x, y] = new TileData(x, y, "floor");

                }
                level.tiles[width - 1, y] = new TileData(x, y, "wall");//side walls
            }
            y = height - 1;
            for (x = 0; x < width; x++)
            {
                level.tiles[x, y] = new TileData(x, y, "wall");//last row gen

            }
        }


        public List<MapGen_Room> GenerateMap(LevelData level)
        {
            rand = new Random();
            this.level = level;

            GenerateMapFrame(level);

            BSPMapNode topNode = new BSPMapNode(height, width, new IntVector2(0, 0), true);

            RecursivelySubdivideNode(topNode, 0); //Create the room structure

            //Get all the leaves of the tree (the actual rooms)
            List<BSPMapNode> leaves = new List<BSPMapNode>();
            GetLeaves(leaves, topNode);

            //Choose 2 random ones to have stairs
            BSPMapNode upStairsRoom = leaves[rand.Next(leaves.Count)];
            BSPMapNode downStairsRoom = upStairsRoom;
            while (downStairsRoom == upStairsRoom || downStairsRoom.IsAdjacent(upStairsRoom))
            {
                downStairsRoom = leaves[rand.Next(leaves.Count)];
            }

            //Find the "critical path" between stairs
            List<BSPMapNode> stairPath = new List<BSPMapNode>();
            bool success = FindPathToNode(stairPath, upStairsRoom, downStairsRoom);
            if (!success)
            {
                throw new Exception("Couldn't find a path between stairs for god knows what reason");
            }

            List<IntVector2> doors = new List<IntVector2>();
            //Add doors to the stairs
            for (int i = 0; i < stairPath.Count - 1; i++)
            {
                doors.Add(AddDoorBetweenRooms(stairPath[i], stairPath[i + 1]));
            }

            //Go thru the list of rooms and try to give each one at least two doors
            foreach (BSPMapNode node in leaves)
            {
                while (node.connectedNodes.Count < 2 && node.connectedNodes.Count < node.adjacentNodes.Count)
                {
                    BSPMapNode nodeToConnect = node.adjacentNodes[rand.Next(node.adjacentNodes.Count)];
                    if (!node.connectedNodes.Contains(nodeToConnect))
                    {
                        doors.Add(AddDoorBetweenRooms(node, nodeToConnect));
                    }
                }
            }


            //Add doors to map
            foreach (IntVector2 door in doors)
            {
                level.tiles[door.X, door.Y] = new TileData(door, "door");
            }

            //Add stairs to map
            IntVector2 upStairsTile = new IntVector2(upStairsRoom.position.X + upStairsRoom.width / 2, upStairsRoom.position.Y + upStairsRoom.height / 2);
            IntVector2 downStairsTile = new IntVector2(downStairsRoom.position.X + downStairsRoom.width / 2, downStairsRoom.position.Y + downStairsRoom.height / 2);

            //level.tiles[upStairsTile.X, upStairsTile.Y] = level.upstairsTile = new TileData(upStairsTile, "upstairs");
            //level.tiles[downStairsTile.X, downStairsTile.Y] = level.downstairsTile = new TileData(downStairsTile, "downstairs");

            //Output rooms as MapGen_Room objects
            List<MapGen_Room> roomList = new List<MapGen_Room>();

            foreach (BSPMapNode node in leaves)
            {
                string roomType = "normal";
                if (node == upStairsRoom)
                    roomType = "upstairsroom";
                else if (node == downStairsRoom)
                    roomType = "downstairsroom";
                roomList.Add(new MapGen_Room(node.position, new IntVector2(node.width, node.height), roomType));
            }

            return roomList;
        }

        private void RecursivelySubdivideNode(BSPMapNode node, int subdivisionDepth)
        {
            //Check if this node is out of subdivision depth
            if (subdivisionDepth > numOfSubdivisions)
                return;

            //Check if this node is too small to subdivide and establish division point
            int divisionPoint = 0;
            if (node.sliceAlongXAxis)
            {
                if (node.height - (minRoomDimensions * 2) < 0)
                    return;
                else
                    divisionPoint = rand.Next(minRoomDimensions, node.height - minRoomDimensions);
            }
            else
            {
                if (node.width - (minRoomDimensions * 2) < 0)
                    return;
                else
                    divisionPoint = rand.Next(minRoomDimensions, node.width - minRoomDimensions);
            }

            //Slice node
            if (node.sliceAlongXAxis)
            {
                node.leftLeaf = new BSPMapNode(divisionPoint, node.width, new IntVector2(node.position.X, node.position.Y), false);
                node.rightLeaf = new BSPMapNode(node.height + 1 - divisionPoint, node.width, new IntVector2(node.position.X, node.position.Y + divisionPoint - 1), false);
            }
            else
            {
                node.leftLeaf = new BSPMapNode(node.height, divisionPoint, new IntVector2(node.position.X, node.position.Y), true);
                node.rightLeaf = new BSPMapNode(node.height, node.width + 1 - divisionPoint, new IntVector2(node.position.X + divisionPoint - 1, node.position.Y), true);
            }

            AddNodeWallsToMap(level, node.leftLeaf);
            AddNodeWallsToMap(level, node.rightLeaf);

            //Figure out neighbors
            //Obviously both leaves are neighbors of each other
            node.leftLeaf.adjacentNodes.Add(node.rightLeaf);
            node.rightLeaf.adjacentNodes.Add(node.leftLeaf);
            //Any nodes adjacent to this one could be adjacent to the leaves, test them
            //Also now that I'm not a leaf nobody cares who I'm adjacent to so get rid of their refs to me
            foreach (BSPMapNode n in node.adjacentNodes)
            {
                if (n.IsAdjacent(node.leftLeaf))
                {
                    node.leftLeaf.adjacentNodes.Add(n);
                    n.adjacentNodes.Add(node.leftLeaf);
                }
                if (n.IsAdjacent(node.rightLeaf))
                {
                    node.rightLeaf.adjacentNodes.Add(n);
                    n.adjacentNodes.Add(node.rightLeaf);
                }
                n.adjacentNodes.Remove(node);
            }

            //Subdivide the largest node next
            if (node.leftLeaf.width > node.rightLeaf.width || node.leftLeaf.height > node.rightLeaf.height)
            {
                RecursivelySubdivideNode(node.leftLeaf, subdivisionDepth + 1);
                if (rand.Next(2) < 1 || subdivisionDepth < 1)
                    RecursivelySubdivideNode(node.rightLeaf, subdivisionDepth + 1);
            }
            else
            {
                RecursivelySubdivideNode(node.rightLeaf, subdivisionDepth + 1);
                if (rand.Next(2) < 1 || subdivisionDepth < 1)
                    RecursivelySubdivideNode(node.leftLeaf, subdivisionDepth + 1);
            }
        }

        private void GetLeaves(List<BSPMapNode> leafList, BSPMapNode node)
        {
            if (node.IsLeaf())
                leafList.Add(node);
            else
            {
                GetLeaves(leafList, node.leftLeaf);
                GetLeaves(leafList, node.rightLeaf);
            }
        }

        private void AddNodeWallsToMap(LevelData l, BSPMapNode node)
        {
            int x = 0;
            int y = 0;

            for (x = node.position.X; x < node.position.X + node.width; x++)
            {
                l.tiles[x, node.position.Y] = new TileData(new IntVector2(x, node.position.Y), "wall");//Top
                l.tiles[x, node.position.Y + node.height - 1] = new TileData(new IntVector2(x, node.position.Y + node.height - 1), "wall");//Bottom
            }

            for (y = node.position.Y; y < node.position.Y + node.height; y++)
            {
                l.tiles[node.position.X, y] = new TileData(new IntVector2(node.position.X, y), "wall");//Left
                l.tiles[node.position.X + node.width - 1, y] = new TileData(new IntVector2(node.position.X + node.width - 1, y), "wall");//Right
            }
        }

        private IntVector2 AddDoorBetweenRooms(BSPMapNode a, BSPMapNode b)
        {
            int xMin = 0;
            int xMax = 0;
            int yMin = 0;
            int yMax = 0;

            int offsetFromCorner = 2;

            IntVector2 doorLoc = null;
            if (a.position.X == b.position.X + b.width - 1) //A to the right of B
            {
                xMin = xMax = a.position.X;
                yMin = Math.Max(a.position.Y + offsetFromCorner, b.position.Y + offsetFromCorner);
                yMax = Math.Min(a.position.Y + a.height - offsetFromCorner, b.position.Y + b.height - offsetFromCorner);
            }
            else if (a.position.X + a.width - 1 <= b.position.X) //To the left
            {
                xMin = xMax = b.position.X;
                yMin = Math.Max(a.position.Y + offsetFromCorner, b.position.Y + offsetFromCorner);
                yMax = Math.Min(a.position.Y + a.height - offsetFromCorner, b.position.Y + b.height - offsetFromCorner);
            }
            else if (a.position.Y >= b.position.Y + b.height - 1) //Below
            {
                yMin = yMax = a.position.Y;
                xMin = Math.Max(a.position.X + offsetFromCorner, b.position.X + offsetFromCorner);
                xMax = Math.Min(a.position.X + a.width - offsetFromCorner, b.position.X + b.width - offsetFromCorner);
            }
            else if (a.position.Y + a.height - 1 <= b.position.Y) //Above
            {
                yMin = yMax = b.position.Y;
                xMin = Math.Max(a.position.X + offsetFromCorner, b.position.X + offsetFromCorner);
                xMax = Math.Min(a.position.X + a.width - offsetFromCorner, b.position.X + b.width - offsetFromCorner);
            }
            else
            {
                return null;
            }
            if (xMin > xMax)
                xMin = xMax;
            if (yMin > yMax)
                yMin = yMax;
            int x = rand.Next(xMin, xMax);
            int y = rand.Next(yMin, yMax);
            doorLoc = new IntVector2(x, y);
            //doorLoc = new IntVector2(0, 0);
            a.connectedNodes.Add(b);
            b.connectedNodes.Add(a);

            return doorLoc;
        }

        private bool FindPathToNode(List<BSPMapNode> path, BSPMapNode current, BSPMapNode destination)
        {
            path.Add(current);
            if (current.adjacentNodes.Contains(destination))
            {
                path.Add(destination);
                return true;
            }
            else
            {
                foreach (BSPMapNode node in current.adjacentNodes)
                {
                    if (!path.Contains(node) && FindPathToNode(path, node, destination))
                    {
                        return true;
                    }
                }
            }
            path.Remove(current);
            return false;
        }
    }

    class BSPMapNode
    {
        public int height;
        public int width;
        public IntVector2 position; //Top left

        public bool sliceAlongXAxis;

        public BSPMapNode leftLeaf;
        public BSPMapNode rightLeaf;

        public List<BSPMapNode> adjacentNodes;
        public List<BSPMapNode> connectedNodes;

        public BSPMapNode(int height, int width, IntVector2 position, bool sliceAlongXAxis)
        {
            this.height = height;
            this.width = width;
            this.position = position;

            this.sliceAlongXAxis = sliceAlongXAxis;
            leftLeaf = null;
            rightLeaf = null;

            adjacentNodes = new List<BSPMapNode>();
            connectedNodes = new List<BSPMapNode>();
        }

        public bool IsLeaf()
        {
            if (leftLeaf == null && rightLeaf == null)
            {
                return true;
            }
            return false;
        }

        public bool IsAdjacent(BSPMapNode other)
        {
            if (position.X >= other.position.X + other.width ||
                position.X + width <= other.position.X ||
                position.Y >= other.position.Y + other.height ||
                position.Y + height <= other.position.Y)
                return false;
            return true;
        }
    }
}
