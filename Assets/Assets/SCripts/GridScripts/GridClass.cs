using UnityEngine;
using CodeMonkey.Utils;
using System;
using Unity.Mathematics;
public class GridClass<TGridObject>
{
    private int width;
    private int height;
    private float cellSize;
    public const int HEAT_MAP_MAX_VALUE = 100;
    public const int HEAT_MAP_MIN_VALUE = 0;

    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs: EventArgs 
    {
        public int x;
        public int y;
    }

    private Vector3 originPosition;
    private TGridObject[,] gridArray;
    
    public  Vector3 GetWolrdPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize+ originPosition;
    }
    public int GetHeight()
      { return height; }
    public int GetWidth()
    { return width; }
    public float GetCellSize()
    { return cellSize; }
    public void GetXY(Vector3 worldPosition,out int x,out int y )
    {
        x=Mathf.FloorToInt((worldPosition- originPosition).x/cellSize);
        y=Mathf.FloorToInt((worldPosition- originPosition).y/cellSize);
    }

    public TGridObject GetValue(Vector3 worldPosition)
    {
        int x;
        int y;
        GetXY(worldPosition, out x, out y);
        return GetValue( x,  y);
    }
    public TGridObject  GetValue (int x,int y)
    {
      if (x>=0&& y>=0&& x<width&&y<height)
        {
            return gridArray[x,y];
        }
        else 
        {
            return default;
        }
    }
    public Vector3 GetWordlPosition(int x,int y)
    {
        return new Vector3 (x,y)*cellSize+ originPosition;
    }
    public void SetValue(Vector3 worldPosition, TGridObject value)
    {
        int x, y;
        GetXY(worldPosition,out x,out y);
        SetValue(x, y, value);
    }

    public void SetValue(int x,int y, TGridObject value)
    {

        if (x >= 0 && y >= 0 && x < width && y < height)
        {
           gridArray[x, y] =value;
          
            if (OnGridValueChanged != null) 
            {
                OnGridValueChanged(this,new OnGridValueChangedEventArgs { x=x, y=y });
            }
        }
    }
   


    public GridClass(int width, int height, float cellSize, Vector3 originPosition,Func<TGridObject> createGridObject )
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        gridArray = new TGridObject[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y <height; y++)
            {
                gridArray[x, y] = createGridObject();
            }
        }

               
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Debug.DrawLine(GetWolrdPosition(x, y), GetWolrdPosition(x, y + 1),Color.cadetBlue,100f);
                Debug.DrawLine(GetWolrdPosition(x, y), GetWolrdPosition(x+1, y),Color.cadetBlue, 100f);
            }
        }
        Debug.DrawLine(GetWolrdPosition(0,height), GetWolrdPosition(width, height), Color.cadetBlue, 100f);
        Debug.DrawLine(GetWolrdPosition(width,0), GetWolrdPosition(width, height), Color.cadetBlue, 100f);
     
      
    }

}
          


