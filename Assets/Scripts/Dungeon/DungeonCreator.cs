// ================================================================================================================================
// File:        DungeonCreator.cs
// Description: Provides functionality to the UI for the user can generate dungeons to their liking
// Description:	Creates a new dungeon floor, based on this article https://gamedevelopment.tutsplus.com/tutorials/create-a-procedurally-generated-dungeon-cave-system--gamedev-10099
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System;
using UnityEngine;
using UnityEngine.UI;

public class DungeonCreator : MonoBehaviour
{
    //Dungeon grid size management
    public GameObject DungeonGridUIWindow;  //UI Window used to setup and manage size of the dungeon grid
    public InputField DungeonWidthInput;    //Input Field for entering in the desired dungeon grid width
    public InputField DungeonHeightInput;   //Input Field for entering in the desired dungeon grid height
    private Vector2 DungeonGridSize = new Vector2(0, 0);    //Dimensions of the tile grid upon which the dungeon will be generated
    private Vector2 DungeonSizeRange = new Vector2(8, 128); //Dimensions of the dungeon grid will be kept within this range
    private bool GridSetup = false; //Remembers once the grid has been setup, so we know to remove the existing grid if told to set it up again

    //Dungeon room placement controls
    public GameObject RoomPlacementUIWindow;    //UI Window used to specify number and size of rooms, then place them in
    public InputField MaxRoomCountInput;  //Input Field for entering in the maximum number of rooms to be generated in
    private int MaxRoomCount = 0;   //Maximum number of rooms that will be created when they are being generated
    private Vector2 RoomCountRange = new Vector2(1, 64);    //Number of rooms allowed will be kept within these values
    public InputField MinRoomSizeInput; //Input Field for entering in the minimum size of the rooms that will be generated
    public InputField MaxRoomSizeInput; //Input Field for entering in the maximum size of the rooms that will be generated
    private int MinRoomSize = 0;    //Minimum size of the rooms that will be generated
    private Vector2 MinRoomSizeRange = new Vector2(2, 6);   //Allowed value range for minimum room size
    private int MaxRoomSize = 0;    //Maximum size of the rooms that will be generated
    private Vector2 MaxRoomSizeRange = new Vector2(3, 10);  //Allowed value range for maximum room size

    //Keeps entered width/height values inside the allowable size range values
    public void GridDimensionsUpdated()
    {
        //Ignore the width field if its empty
        if(DungeonWidthInput.text != "")
        {
            //Fetch and store the width value that was entered in
            DungeonGridSize.x = Convert.ToInt32(DungeonWidthInput.text);
            //Make sure it says within the allowed size range
            DungeonGridSize.x = Mathf.Clamp(DungeonGridSize.x, DungeonSizeRange.x, DungeonSizeRange.y);
            //Update the UI with the new width value incause it was just clamped to stay inside allowed range
            DungeonWidthInput.text = DungeonGridSize.x.ToString();
        }
        //Do everything the same with the height field
        if(DungeonHeightInput.text != "")
        {
            DungeonGridSize.y = Convert.ToInt32(DungeonHeightInput.text);
            DungeonGridSize.y = Mathf.Clamp(DungeonGridSize.y, DungeonSizeRange.x, DungeonSizeRange.y);
            DungeonHeightInput.text = DungeonGridSize.y.ToString();
        }
    }

    //Sets up the dungeon grid with whatever size values have been entered in
    public void ClickSetupDungeonGrid()
    {
        //Ignore this if either of the size values have yet to be specified
        if (DungeonGridSize.x == 0 || DungeonGridSize.y == 0)
            return;

        //Clean up the current grid if one already exists
        if (GridSetup)
            Dungeon.Instance.ClearDungeon();

        //Now initialize the grid as normal, and set the room placement window to be seen
        Dungeon.Instance.InitializeGrid((int)DungeonGridSize.x, (int)DungeonGridSize.y);
        RoomPlacementUIWindow.SetActive(true);
        GridSetup = true;
    }

    //Keeps entered room count value inside the allowed value range
    public void RoomCountUpdated()
    {
        //Fetch the new room count, clamp it within allowed value range, then update the UI
        MaxRoomCount = Convert.ToInt32(MaxRoomCountInput.text);
        MaxRoomCount = (int)Mathf.Clamp(MaxRoomCount, RoomCountRange.x, RoomCountRange.y);
        MaxRoomCountInput.text = MaxRoomCount.ToString();
    }

    //Keeps entered min/max room size values in acceptable range
    public void RoomSizeUpdated()
    {
        //Min room size
        if(MinRoomSizeInput.text != "")
        {
            //Clamp minimum room size, make sure its less than or equal to max size
            MinRoomSize = Convert.ToInt32(MinRoomSizeInput.text);
            MinRoomSize = (int)Mathf.Clamp(MinRoomSize, MinRoomSizeRange.x, MinRoomSizeRange.y);
            if (MaxRoomSize != 0 && MinRoomSize > MaxRoomSize)
                MinRoomSize = MaxRoomSize;
            MinRoomSizeInput.text = MinRoomSize.ToString();
        }

        if(MaxRoomSizeInput.text != "")
        {
            MaxRoomSize = Convert.ToInt32(MaxRoomSizeInput.text);
            MaxRoomSize = (int)Mathf.Clamp(MaxRoomSize, MaxRoomSizeRange.x, MaxRoomSizeRange.y);
            if (MinRoomSize != 0 && MaxRoomSize < MinRoomSize)
                MaxRoomSize = MinRoomSize;
            MaxRoomSizeInput.text = MaxRoomSize.ToString();
        }
    }

    //Places down rooms onto the dungeon grid
    public void ClickCreateRooms()
    {
        //Make sure fields have been filled out, and contain valid values
        bool FieldsEmpty = MaxRoomCount == 0 || MinRoomSize == 0 || MaxRoomSize == 0;
        bool SizesValid = MinRoomSize <= MaxRoomSize && MaxRoomSize >= MinRoomSize;

        //Generate the rooms if all validation tests have passed
        if (!FieldsEmpty && SizesValid)
            Dungeon.Instance.PlaceRooms(MaxRoomCount, MinRoomSize, MaxRoomSize);
    }
}