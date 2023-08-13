#include <graphics.h>	
#include <stdlib.h>	
#include <stdio.h>	
#include <conio.h>	
#include <math.h>	
// this function initializes graphics mode	
// it will work only if you're using Borland C++ compiler & BGI drivers	
// if you're using another compiler you should overwrite body of this function	
void init_gr(void)	
{	
   /* request autodetection */	
   int gdriver = DETECT, gmode, errorcode;	
   /* initialize graphics mode */	
   initgraph(&gdriver, &gmode, "");	
   /* read result of initialization */	
   errorcode = graphresult();	
   if (errorcode != grOk)    /* an error occurred */	
   {	
      printf("Graphics error: %s\n", grapherrormsg(errorcode));	
      printf("Press any key to halt:");	
      getch();	
      exit(1);               /* return with error code */	
   }	
}	
// this function shuts graphics mode down	
// it will work only if you're using Borland C++ compiler & BGI drivers	
// if you're using another compiler you should overwrite body of this function	
void end_gr(void)	
{	
  closegraph();	
}	
// this function puts pixel on the screen in (x,y) position using color 'color'	
// it will work only if you're using Borland C++ compiler & BGI drivers	
// if you're using another compiler you should overwrite body of this function	
void PutPixel(int x, int y, int color)	
{	
  putpixel(x,y,color);	
}	
float const ratio=1.0; // you can change this to draw ellipses	
// This function plots points that belongs to the circle	
// It recieves offsets from center for the fist quadrant	
// and plots symmetrical points in all four quadrants	
void plot_circle(int x,int y, int x_center, int y_center, int color)	
{	
  int x_start,y_start,x_end,y_end,x1,y1;	
  // the distanse between start and end can be greater than 1 if ratio!=1	
  y_start=y*ratio;	
  y_end=(y+1)*ratio;	
  x_start=x*ratio;	
  x_end=(x+1)*ratio;	
  for (x1=x_start;x1<x_end;++x1)	
  {	
    // plot points in all four quadrants	
    PutPixel(x1+x_center,y+y_center,color);	
    PutPixel(x1+x_center,y_center-y,color);	
    PutPixel(x_center-x1,y+y_center,color);	
    PutPixel(x_center-x1,y_center-y,color);	
  }	
  for (y1=y_start;y1<y_end;++y1)	
  {	
    // plot points in all four quadrants	
    PutPixel(y1+x_center,x+y_center,color);	
    PutPixel(y1+x_center,y_center-x,color);	
    PutPixel(x_center-y1,x+y_center,color);	
    PutPixel(x_center-y1,y_center-x,color);	
  }	
}	
// This is main function that draws circle using function	
void Circle(int x1,int y1,int radius, int color)	
{	
  int x,y,delta;	
//     Y    *              we start from * and increase x step by step	
//          |              decreasing y when needed	
//          |	
//          |	
// --------------------	
//          |         X	
//          |	
//          |	
//          |	
  y=radius;	
  delta=3-2*radius; // delta is an error	
  // calculate values for first quadrant	
  for (x=0;x<y;x++) // x is a main axe	
  {	
    // plot points symmetrically in all quadrants	
    plot_circle(x,y,x1,y1,color);	
    if (delta<0) delta+=4*x+6;	
    else	
    {	
      delta+=4*(x-y)+10;	
      y--; // it's time to decrease y	
    }	
  }	
  x=y;	
  if (y!=0) plot_circle(x,y,x1,y1,color);	
}	
int main(void)	
{	
  // initializing graphics mode	
  init_gr();	
  /* examples */	
  Circle(200,200,100,14);	
  Circle(300,200,100,15);	
  Circle(400,200,100,13);	
  Circle(250,100,100,12);	
  Circle(350,100,100,11);	
  Circle(50,400,25,2);	
  Circle(500,400,25,2);	
  /* clean up */	
  getch();	
  end_gr();	
  return 0;	
}