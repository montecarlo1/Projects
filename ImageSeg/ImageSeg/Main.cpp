#include <iostream>  
#include <opencv2/core/core.hpp>  
#include <opencv2/highgui/highgui.hpp>  
#include "opencv2/imgproc/imgproc.hpp"  
#include <stdio.h>  
#include "cv.h"
#include "cxcore.h"
#include <map>
using namespace cv;
using namespace std;
typedef pair<int, int> PAIR;

ostream& operator<<(ostream& out, const PAIR& p) 
{
	return out << p.first << "\t" << p.second;
}
//水平投影
vector<Mat> horizontalProjectionMat(Mat srcImg)
{
	Mat binImg;
	//均值滤波
	boxFilter(srcImg, binImg, -1, Size(5, 5));
	//threshold(binImg, binImg, 0, 255, CV_THRESH_OTSU)
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


//垂直投影  
vector<Mat> verticalProjectionMat(Mat srcImg)
{
	Mat binImg;
	//CV_THRESH_MASK
	boxFilter(srcImg, binImg, -1, Size(5, 5));
	threshold(binImg, binImg, 0, 255, CV_THRESH_OTSU);
	int perPixelValue;//每个像素的值  
	int width = srcImg.cols;
	int height = srcImg.rows;
	int* projectValArry = new int[width];                  //创建用于储存每列白色像素个数的数组  
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

/**************************************************************** 
功能：      对前景目标（如字符）进行划分，将各个字符的轮廓矩形返回  
参数：      img：输入等待分割的图像 
            int areaThreshold：面积阈值，当分割后的字符小于这个值时，会擦除该字符，用于去除噪音等小区域杂点干扰 
注    ：   只能处理二值图像 
返回值：   返回分割字符的矩形框 
***************************************************************/  
vector<CvRect> ObjectSegment(IplImage* img, int areaThreshold)
{  
    vector<CvRect> vecRoughRECT;    //粗略对象轮廓的矩形向量数组      
    vector<CvRect> vecRECT;         //精化后对象轮廓的矩形向量数组      
    vector<CvRect> vecRECTBig;     //存放精化对象区域中大的的矩形框      
    //清空用来表示每个对象的vector      
    vecRoughRECT.clear();  
    vecRECT.clear();  
    vecRECTBig.clear();  
  
    int nTop, nBttom;   //整体前景区域的上下边界      
    int nObjCnt = 0;      //对象数目      
  
    //从上向下扫描，找到整体区域的前景的上边界      
    for (int i = 0; i < img->height; i++)  
    {  
        for (int j = 0; j < img->width; j++)  
        {  
            double pixel = cvGetReal2D(img, i, j);  
            if (int(pixel) == 0)  
            {  
                nTop = i;  
                i = img->height;   //对i赋大值，使得在break跳出内存循环后，直接在跳出外层循环      
                break;  
            }  
        }  
    }  
  
    //从下向上扫描，找到整体区域的前景的下边界      
    for (int i = img->height - 1; i >= 0; i--)  
    {  
        for (int j = 0; j < img->width; j++)  
        {  
            double pixel = cvGetReal2D(img, i, j);  
            if (int(pixel) == 0)  
            {  
                nBttom = i;  
                i = -1;   //对i赋小值，使得在break跳出内存循环后，直接在跳出外层循环      
                break;  
            }  
        }  
    }  
  
    bool bStartSeg = false;      //是否已经开始某一个对象的分割      
    bool bBlackInCol;             //某一列中是否包含黑色像素      
  
    cvSaveImage("C:\\Users\\Administrator\\Desktop\\rect0.jpg", img);     
    return vecRECTBig;  
}

int BasicGlobalThreshold(int*pg, int start, int end)
{                                           //  基本全局阈值法
	int  i, t, t1, t2, k1, k2;
	double u, u1, u2;
	t = 0;
	u = 0;
	for (i = start; i<end; i++)
	{
		t += pg[i];
		u += i*pg[i];
	}
	k2 = (int)(u / t);                          //  计算此范围灰度的平均值    
	do
	{
		k1 = k2;
		t1 = 0;
		u1 = 0;
		for (i = start; i <= k1; i++)
		{             //  计算低灰度组的累加和
			t1 += pg[i];
			u1 += i*pg[i];
		}
		t2 = t - t1;
		u2 = u - u1;
		if (t1)
			u1 = u1 / t1;                     //  计算低灰度组的平均值
		else
			u1 = 0;
		if (t2)
			u2 = u2 / t2;                     //  计算高灰度组的平均值
		else
			u2 = 0;
		k2 = (int)((u1 + u2) / 2);                 //  得到新的阈值估计值
	} while (k1 != k2);                           //  数据未稳定，继续
											   //cout<<"The Threshold of this Image in BasicGlobalThreshold is:"<<k1<<endl;
	return(k1);                              //  返回阈值
}
int main(int argc, char* argv[])
{
	//Mat srcImg = imread("F:\\45.jpg", 0);//将读入的彩色图像以灰度图像读入
	//Mat res, dst;
	//原图
	/*namedWindow("original image", 1);
	imshow("orginal image", srcImg);
	res = srcImg.clone();*/
	//对原图像进行二值化处理,选择30，the threshold value is 200
	/*threshold(srcImg, res, 30, 200.0,CV_THRESH_OTSU);
	namedWindow("二值化图像");
	imshow("二值化图像", res);*/
    //对图像进行灰度处理
	//cvtColor(srcImg, dst, CV_BGR2GRAY);
	//namedWindow("GRAY", WINDOW_AUTOSIZE);
	//imshow("GRAY", dst);
	//cout << dst.channels() << endl;
	//对图像进行降噪和归一化处理
	//char szName[300] = { 0 };
	//vector<Mat> b = verticalProjectionMat(srcImg);//先进行垂直投影     
	//for (int i = 0; i < b.size(); i++)
	//{
	//	vector<Mat> a = horizontalProjectionMat(b[i]);//水平投影  
	//	sprintf_s(szName, "F:\\mypicture2\\%d.jpg", i);
	//	for (int j = 0; j < a.size(); j++)
	//	{
	//		imshow(szName, a[j]);
	//		IplImage img = IplImage(a[j]);
	//		cvSaveImage(szName, &img);      //保存切分的结果  
	//	}
	//}
	//vector<Mat> a = horizontalProjectionMat(srcImg);
	//for (int i = 0; i < a.size(); i++)
	//{
	//    vector<Mat> b = verticalProjectionMat(a[i]);
	//    for (int j = 0; j<b.size();j++)
	//    {
	//         sprintf_s(szName, "F:\\mypicture2\\%d.jpg", j);
	//		 imshow(szName, b[j]);
	//		 IplImage img = IplImage(b[j]);
	//	     cvSaveImage(szName, &img);//保存切分的结果  
	//    }
	//}	
	//IplImage* imgSrc = cvLoadImage("F:\\321.jpg", CV_LOAD_IMAGE_COLOR);
	//IplImage* img_gray = cvCreateImage(cvGetSize(imgSrc), IPL_DEPTH_8U, 1);
	//cvCvtColor(imgSrc, img_gray, CV_BGR2GRAY);
	//cvThreshold(img_gray, img_gray, 100, 255, CV_THRESH_BINARY_INV);// CV_THRESH_BINARY_INV使得背景为黑色，字符为白色，这样找到的最外层才是字符的最外层  
	//cvShowImage("ThresholdImg", img_gray);
	//CvSeq* contours = NULL;
	//CvMemStorage* storage = cvCreateMemStorage(0);
	// 上面源图片有瑕疵可以用腐蚀，膨胀来祛除  
	//int count = cvFindContours(img_gray, storage, &contours, sizeof(CvContour), CV_RETR_EXTERNAL);
	//printf("轮廓个数：%d", count);
	//int idx = 0;
	//char szName[56] = { 0 };
	//int tempCount = 0;
	//for (CvSeq* c = contours; c != NULL; c = c->h_next) {
	//	CvRect rc = cvBoundingRect(c, 0);
	//	cvDrawRect(imgSrc, cvPoint(rc.x, rc.y), cvPoint(rc.x + rc.width, rc.y + rc.height), CV_RGB(255, 0, 0));
	//	IplImage* imgNo = cvCreateImage(cvSize(rc.width, rc.height), IPL_DEPTH_8U, 3);
	//	cvSetImageROI(imgSrc, rc);

	//	cvCopy(imgSrc, imgNo);
	//	cvResetImageROI(imgSrc);
	//	sprintf_s(szName, "F:\\mypicture\\%d.jpg", idx++);
	//	cvNamedWindow(szName);

	//	cvShowImage(szName, imgNo); //如果想切割出来的图像从左到右排序，或从上到下，可以比较rc.x,rc.y;  
	//
	//	cvSaveImage(szName, imgNo);
	//	cvReleaseImage(&imgNo);
	//}

	//cvNamedWindow("src");
	//cvShowImage("src", imgSrc);
	//cvWaitKey(0);
	//cvReleaseMemStorage(&storage);
	//cvReleaseImage(&imgSrc);
	//cvReleaseImage(&img_gray);
	//cvDestroyAllWindows();


	//IplImage *res, *dst;
	//CvRect rect;
	//map<int, int> adjust_map;
	//adjust_map[300] = 70;
	//adjust_map[683] = 35;
	//adjust_map.insert(make_pair(883, 240));
	//adjust_map.insert(make_pair(1678, 300));
	//int x[] = {300,683,883,1678};
	//int width[] = {70,35,240,300};

	//for (map<int, int>::iterator iter = adjust_map.begin(); iter != adjust_map.end(); ++iter) {
	//	//sprintf_s(*iter);
	//}
	/*int name[] = { 2,3,4 };

	char szName[100] = { 0 };
	for (int i = 0; i < 4; i++)
	{
		rect.x = x[i];
		rect.width = width[i];		
		res = cvLoadImage("F:\\00.jpg");
		rect.height = 40;
		rect.y = 320;
		dst = cvCreateImage(cvSize(rect.width, rect.height), 8, 3);																  
		cvNamedWindow("res", CV_WINDOW_AUTOSIZE);
		cvNamedWindow("dst", CV_WINDOW_AUTOSIZE);
		cvSetImageROI(res, rect);
		cvCopy(res, dst);
		cvResetImageROI(res);
		cvShowImage("res", res);
		cvShowImage("dst", dst);
		sprintf_s(szName, "F:\\mypicture\\%d.jpg", i);
		cvSaveImage(szName, dst);
	}
	cvWaitKey(0);
	cvDestroyWindow("res");
	cvDestroyWindow("dst");
	cvReleaseImage(&res);
	cvReleaseImage(&dst);	*/
	return 0;
}