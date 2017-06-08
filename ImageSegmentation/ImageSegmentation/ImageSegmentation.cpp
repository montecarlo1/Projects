// ImageSegmentation.cpp : Defines the entry point for the console application.
//

#include <stdafx.h>  
#include "opencv2/imgproc/imgproc.hpp"  
#include "opencv2/highgui/highgui.hpp"  
#include <iostream>  
#include <stdio.h>  
using namespace cv;
using namespace std;

vector<Mat> horizontalProjectionMat(Mat srcImg)//水平投影  
{
	Mat binImg;
	blur(srcImg, binImg, Size(3, 3));
	threshold(binImg, binImg, 0, 255, CV_THRESH_OTSU);
	int perPixelValue = 0;//每个像素的值  
	int width = srcImg.cols;
	int height = srcImg.rows;
	int* projectValArry = new int[height];//创建一个储存每行白色像素个数的数组  
	memset(projectValArry, 0, height * 4);//初始化数组  
	for (int col = 0; col < height; col++)//遍历每个像素点  
	{
		for (int row = 0; row < width; row++)
		{
			perPixelValue = binImg.at<uchar>(col, row);
			if (perPixelValue == 0)//如果是白底黑字  
			{
				projectValArry[col]++;
			}
		}
	}
	Mat horizontalProjectionMat(height, width, CV_8UC1);//创建画布  
	for (int i = 0; i < height; i++)
	{
		for (int j = 0; j < width; j++)
		{
			perPixelValue = 255;
			horizontalProjectionMat.at<uchar>(i, j) = perPixelValue;//设置背景为白色  
		}
	}
	for (int i = 0; i < height; i++)//水平直方图  
	{
		for (int j = 0; j < projectValArry[i]; j++)
		{
			perPixelValue = 0;
			horizontalProjectionMat.at<uchar>(i, width - 1 - j) = perPixelValue;//设置直方图为黑色  
		}
	}
	vector<Mat> roiList;//用于储存分割出来的每个字符  
	int startIndex = 0;//记录进入字符区的索引  
	int endIndex = 0;//记录进入空白区域的索引  
	bool inBlock = false;//是否遍历到了字符区内  
	for (int i = 0; i <srcImg.rows; i++)
	{
		if (!inBlock && projectValArry[i] != 0)//进入字符区  
		{
			inBlock = true;
			startIndex = i;
		}
		else if (inBlock && projectValArry[i] == 0)//进入空白区  
		{
			endIndex = i;
			inBlock = false;
			Mat roiImg = srcImg(Range(startIndex, endIndex + 1), Range(0, srcImg.cols));//从原图中截取有图像的区域  
			roiList.push_back(roiImg);
		}
	}
	delete[] projectValArry;
	return roiList;
}
vector<Mat> verticalProjectionMat(Mat srcImg)//垂直投影  
{
	Mat binImg;
	blur(srcImg, binImg, Size(3, 3));
	threshold(binImg, binImg, 0, 255, CV_THRESH_OTSU);
	int perPixelValue;//每个像素的值  
	int width = srcImg.cols;
	int height = srcImg.rows;
	int* projectValArry = new int[width];//创建用于储存每列白色像素个数的数组  
	memset(projectValArry, 0, width * 4);//初始化数组  
	for (int col = 0; col < width; col++)
	{
		for (int row = 0; row < height; row++)
		{
			perPixelValue = binImg.at<uchar>(row, col);
			if (perPixelValue == 0)//如果是白底黑字  
			{
				projectValArry[col]++;
			}
		}
	}
	Mat verticalProjectionMat(height, width, CV_8UC1);//垂直投影的画布  
	for (int i = 0; i < height; i++)
	{
		for (int j = 0; j < width; j++)
		{
			perPixelValue = 255;  //背景设置为白色  
			verticalProjectionMat.at<uchar>(i, j) = perPixelValue;
		}
	}
	for (int i = 0; i < width; i++)//垂直投影直方图  
	{
		for (int j = 0; j < projectValArry[i]; j++)
		{
			perPixelValue = 0;  //直方图设置为黑色    
			verticalProjectionMat.at<uchar>(height - 1 - j, i) = perPixelValue;
		}
	}
	imshow("垂直投影", verticalProjectionMat);
	cvWaitKey(0);
	vector<Mat> roiList;//用于储存分割出来的每个字符  
	int startIndex = 0;//记录进入字符区的索引  
	int endIndex = 0;//记录进入空白区域的索引  
	bool inBlock = false;//是否遍历到了字符区内  
	for (int i = 0; i < srcImg.cols; i++)//cols=width  
	{
		if (!inBlock && projectValArry[i] != 0)//进入字符区  
		{
			inBlock = true;
			startIndex = i;
		}
		else if (projectValArry[i] == 0 && inBlock)//进入空白区  
		{
			endIndex = i;
			inBlock = false;
			Mat roiImg = srcImg(Range(0, srcImg.rows), Range(startIndex, endIndex + 1));
			roiList.push_back(roiImg);
		}
	}
	delete[] projectValArry;
	return roiList;
}
int main(int argc, char* argv[])
{
	Mat srcImg = imread("E:\\b.png", 0);//读入原图像  
	char szName[30] = { 0 };
	vector<Mat> b = verticalProjectionMat(srcImg);//先进行垂直投影     
	for (int i = 0; i < b.size(); i++)
	{
		vector<Mat> a = horizontalProjectionMat(b[i]);//水平投影  
		sprintf(szName, "E:\\picture\\%d.jpg", i);
		for (int j = 0; j < a.size(); j++)
		{
			imshow(szName, a[j]);
			IplImage img = IplImage(a[j]);
			cvSaveImage(szName, &img);//保存切分的结果  
		}
	}
	/*
	vector<Mat> a = horizontalProjectionMat(srcImg);
	char szName[30] = { 0 };
	for (int i = 0; i < a.size(); i++)
	{
	vector<Mat> b = verticalProjectionMat(a[i]);
	for (int j = 0; j<b.size();j++)
	{
	sprintf(szName, "E:\\%d.jpg", j);
	imshow(szName, b[j]);
	}
	}
	*/
	cvWaitKey(0);
	getchar();
	return 0;
}