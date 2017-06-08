// ImageSegmentation.cpp : Defines the entry point for the console application.
//

#include <stdafx.h>  
#include "opencv2/imgproc/imgproc.hpp"  
#include "opencv2/highgui/highgui.hpp"  
#include <iostream>  
#include <stdio.h>  
using namespace cv;
using namespace std;

vector<Mat> horizontalProjectionMat(Mat srcImg)//ˮƽͶӰ  
{
	Mat binImg;
	blur(srcImg, binImg, Size(3, 3));
	threshold(binImg, binImg, 0, 255, CV_THRESH_OTSU);
	int perPixelValue = 0;//ÿ�����ص�ֵ  
	int width = srcImg.cols;
	int height = srcImg.rows;
	int* projectValArry = new int[height];//����һ������ÿ�а�ɫ���ظ���������  
	memset(projectValArry, 0, height * 4);//��ʼ������  
	for (int col = 0; col < height; col++)//����ÿ�����ص�  
	{
		for (int row = 0; row < width; row++)
		{
			perPixelValue = binImg.at<uchar>(col, row);
			if (perPixelValue == 0)//����ǰ׵׺���  
			{
				projectValArry[col]++;
			}
		}
	}
	Mat horizontalProjectionMat(height, width, CV_8UC1);//��������  
	for (int i = 0; i < height; i++)
	{
		for (int j = 0; j < width; j++)
		{
			perPixelValue = 255;
			horizontalProjectionMat.at<uchar>(i, j) = perPixelValue;//���ñ���Ϊ��ɫ  
		}
	}
	for (int i = 0; i < height; i++)//ˮƽֱ��ͼ  
	{
		for (int j = 0; j < projectValArry[i]; j++)
		{
			perPixelValue = 0;
			horizontalProjectionMat.at<uchar>(i, width - 1 - j) = perPixelValue;//����ֱ��ͼΪ��ɫ  
		}
	}
	vector<Mat> roiList;//���ڴ���ָ������ÿ���ַ�  
	int startIndex = 0;//��¼�����ַ���������  
	int endIndex = 0;//��¼����հ����������  
	bool inBlock = false;//�Ƿ���������ַ�����  
	for (int i = 0; i <srcImg.rows; i++)
	{
		if (!inBlock && projectValArry[i] != 0)//�����ַ���  
		{
			inBlock = true;
			startIndex = i;
		}
		else if (inBlock && projectValArry[i] == 0)//����հ���  
		{
			endIndex = i;
			inBlock = false;
			Mat roiImg = srcImg(Range(startIndex, endIndex + 1), Range(0, srcImg.cols));//��ԭͼ�н�ȡ��ͼ�������  
			roiList.push_back(roiImg);
		}
	}
	delete[] projectValArry;
	return roiList;
}
vector<Mat> verticalProjectionMat(Mat srcImg)//��ֱͶӰ  
{
	Mat binImg;
	blur(srcImg, binImg, Size(3, 3));
	threshold(binImg, binImg, 0, 255, CV_THRESH_OTSU);
	int perPixelValue;//ÿ�����ص�ֵ  
	int width = srcImg.cols;
	int height = srcImg.rows;
	int* projectValArry = new int[width];//�������ڴ���ÿ�а�ɫ���ظ���������  
	memset(projectValArry, 0, width * 4);//��ʼ������  
	for (int col = 0; col < width; col++)
	{
		for (int row = 0; row < height; row++)
		{
			perPixelValue = binImg.at<uchar>(row, col);
			if (perPixelValue == 0)//����ǰ׵׺���  
			{
				projectValArry[col]++;
			}
		}
	}
	Mat verticalProjectionMat(height, width, CV_8UC1);//��ֱͶӰ�Ļ���  
	for (int i = 0; i < height; i++)
	{
		for (int j = 0; j < width; j++)
		{
			perPixelValue = 255;  //��������Ϊ��ɫ  
			verticalProjectionMat.at<uchar>(i, j) = perPixelValue;
		}
	}
	for (int i = 0; i < width; i++)//��ֱͶӰֱ��ͼ  
	{
		for (int j = 0; j < projectValArry[i]; j++)
		{
			perPixelValue = 0;  //ֱ��ͼ����Ϊ��ɫ    
			verticalProjectionMat.at<uchar>(height - 1 - j, i) = perPixelValue;
		}
	}
	imshow("��ֱͶӰ", verticalProjectionMat);
	cvWaitKey(0);
	vector<Mat> roiList;//���ڴ���ָ������ÿ���ַ�  
	int startIndex = 0;//��¼�����ַ���������  
	int endIndex = 0;//��¼����հ����������  
	bool inBlock = false;//�Ƿ���������ַ�����  
	for (int i = 0; i < srcImg.cols; i++)//cols=width  
	{
		if (!inBlock && projectValArry[i] != 0)//�����ַ���  
		{
			inBlock = true;
			startIndex = i;
		}
		else if (projectValArry[i] == 0 && inBlock)//����հ���  
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
	Mat srcImg = imread("E:\\b.png", 0);//����ԭͼ��  
	char szName[30] = { 0 };
	vector<Mat> b = verticalProjectionMat(srcImg);//�Ƚ��д�ֱͶӰ     
	for (int i = 0; i < b.size(); i++)
	{
		vector<Mat> a = horizontalProjectionMat(b[i]);//ˮƽͶӰ  
		sprintf(szName, "E:\\picture\\%d.jpg", i);
		for (int j = 0; j < a.size(); j++)
		{
			imshow(szName, a[j]);
			IplImage img = IplImage(a[j]);
			cvSaveImage(szName, &img);//�����зֵĽ��  
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