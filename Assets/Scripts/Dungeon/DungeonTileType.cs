// ================================================================================================================================
// File:        DungeonTileType.cs
// Description:	enumerator definition for the different types of tiles available in the dungeons
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

public enum DungeonTileType
{
    Void = 0,   //EmptyTile
    Floor = 1,  //RoomMiddleTile
    TopWall = 2,    //RoomTopTile
    TopRightWall = 3,   //RoomTopRightTile
    RightWall = 4,  //RoomRightTile
    BottomRightWall = 5,    //RoomBottomRightTile
    BottomWall = 6, //RoomBottomTile
    BottomLeftWall = 7, //RoomBottomLeftTile
    LeftWall = 8,   //RoomLeftTile
    TopLeftWall = 9,    //RoomTopLeftTile
    Door = 10   //RoomDoor
}