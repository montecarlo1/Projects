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
//ˮƽͶӰ
vector<Mat> horizontalProjectionMat(Mat srcImg)
{
	Mat binImg;
	//��ֵ�˲�
	boxFilter(srcImg, binImg, -1, Size(5, 5));
	//threshold(binImg, binImg, 0, 255, CV_THRESH_OTSU)
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


//��ֱͶӰ  
vector<Mat> verticalProjectionMat(Mat srcImg)
{
	Mat binImg;
	//CV_THRESH_MASK
	boxFilter(srcImg, binImg, -1, Size(5, 5));
	threshold(binImg, binImg, 0, 255, CV_THRESH_OTSU);
	int perPixelValue;//ÿ�����ص�ֵ  
	int width = srcImg.cols;
	int height = srcImg.rows;
	int* projectValArry = new int[width];                  //�������ڴ���ÿ�а�ɫ���ظ���������  
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

/**************************************************************** 
���ܣ�      ��ǰ��Ŀ�꣨���ַ������л��֣��������ַ����������η���  
������      img������ȴ��ָ��ͼ�� 
            int areaThreshold�������ֵ�����ָ����ַ�С�����ֵʱ����������ַ�������ȥ��������С�����ӵ���� 
ע    ��   ֻ�ܴ����ֵͼ�� 
����ֵ��   ���طָ��ַ��ľ��ο� 
***************************************************************/  
vector<CvRect> ObjectSegment(IplImage* img, int areaThreshold)
{  
    vector<CvRect> vecRoughRECT;    //���Զ��������ľ�����������      
    vector<CvRect> vecRECT;         //��������������ľ�����������      
    vector<CvRect> vecRECTBig;     //��ž������������д�ĵľ��ο�      
    //���������ʾÿ�������vector      
    vecRoughRECT.clear();  
    vecRECT.clear();  
    vecRECTBig.clear();  
  
    int nTop, nBttom;   //����ǰ����������±߽�      
    int nObjCnt = 0;      //������Ŀ      
  
    //��������ɨ�裬�ҵ����������ǰ�����ϱ߽�      
    for (int i = 0; i < img->height; i++)  
    {  
        for (int j = 0; j < img->width; j++)  
        {  
            double pixel = cvGetReal2D(img, i, j);  
            if (int(pixel) == 0)  
            {  
                nTop = i;  
                i = img->height;   //��i����ֵ��ʹ����break�����ڴ�ѭ����ֱ�����������ѭ��      
                break;  
            }  
        }  
    }  
  
    //��������ɨ�裬�ҵ����������ǰ�����±߽�      
    for (int i = img->height - 1; i >= 0; i--)  
    {  
        for (int j = 0; j < img->width; j++)  
        {  
            double pixel = cvGetReal2D(img, i, j);  
            if (int(pixel) == 0)  
            {  
                nBttom = i;  
                i = -1;   //��i��Сֵ��ʹ����break�����ڴ�ѭ����ֱ�����������ѭ��      
                break;  
            }  
        }  
    }  
  
    bool bStartSeg = false;      //�Ƿ��Ѿ���ʼĳһ������ķָ�      
    bool bBlackInCol;             //ĳһ�����Ƿ������ɫ����      
  
    cvSaveImage("C:\\Users\\Administrator\\Desktop\\rect0.jpg", img);     
    return vecRECTBig;  
}

int BasicGlobalThreshold(int*pg, int start, int end)
{                                           //  ����ȫ����ֵ��
	int  i, t, t1, t2, k1, k2;
	double u, u1, u2;
	t = 0;
	u = 0;
	for (i = start; i<end; i++)
	{
		t += pg[i];
		u += i*pg[i];
	}
	k2 = (int)(u / t);                          //  ����˷�Χ�Ҷȵ�ƽ��ֵ    
	do
	{
		k1 = k2;
		t1 = 0;
		u1 = 0;
		for (i = start; i <= k1; i++)
		{             //  ����ͻҶ�����ۼӺ�
			t1 += pg[i];
			u1 += i*pg[i];
		}
		t2 = t - t1;
		u2 = u - u1;
		if (t1)
			u1 = u1 / t1;                     //  ����ͻҶ����ƽ��ֵ
		else
			u1 = 0;
		if (t2)
			u2 = u2 / t2;                     //  ����߻Ҷ����ƽ��ֵ
		else
			u2 = 0;
		k2 = (int)((u1 + u2) / 2);                 //  �õ��µ���ֵ����ֵ
	} while (k1 != k2);                           //  ����δ�ȶ�������
											   //cout<<"The Threshold of this Image in BasicGlobalThreshold is:"<<k1<<endl;
	return(k1);                              //  ������ֵ
}
int main(int argc, char* argv[])
{
	//Mat srcImg = imread("F:\\45.jpg", 0);//������Ĳ�ɫͼ���ԻҶ�ͼ�����
	//Mat res, dst;
	//ԭͼ
	/*namedWindow("original image", 1);
	imshow("orginal image", srcImg);
	res = srcImg.clone();*/
	//��ԭͼ����ж�ֵ������,ѡ��30��the threshold value is 200
	/*threshold(srcImg, res, 30, 200.0,CV_THRESH_OTSU);
	namedWindow("��ֵ��ͼ��");
	imshow("��ֵ��ͼ��", res);*/
    //��ͼ����лҶȴ���
	//cvtColor(srcImg, dst, CV_BGR2GRAY);
	//namedWindow("GRAY", WINDOW_AUTOSIZE);
	//imshow("GRAY", dst);
	//cout << dst.channels() << endl;
	//��ͼ����н���͹�һ������
	//char szName[300] = { 0 };
	//vector<Mat> b = verticalProjectionMat(srcImg);//�Ƚ��д�ֱͶӰ     
	//for (int i = 0; i < b.size(); i++)
	//{
	//	vector<Mat> a = horizontalProjectionMat(b[i]);//ˮƽͶӰ  
	//	sprintf_s(szName, "F:\\mypicture2\\%d.jpg", i);
	//	for (int j = 0; j < a.size(); j++)
	//	{
	//		imshow(szName, a[j]);
	//		IplImage img = IplImage(a[j]);
	//		cvSaveImage(szName, &img);      //�����зֵĽ��  
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
	//	     cvSaveImage(szName, &img);//�����зֵĽ��  
	//    }
	//}	
	//IplImage* imgSrc = cvLoadImage("F:\\321.jpg", CV_LOAD_IMAGE_COLOR);
	//IplImage* img_gray = cvCreateImage(cvGetSize(imgSrc), IPL_DEPTH_8U, 1);
	//cvCvtColor(imgSrc, img_gray, CV_BGR2GRAY);
	//cvThreshold(img_gray, img_gray, 100, 255, CV_THRESH_BINARY_INV);// CV_THRESH_BINARY_INVʹ�ñ���Ϊ��ɫ���ַ�Ϊ��ɫ�������ҵ������������ַ��������  
	//cvShowImage("ThresholdImg", img_gray);
	//CvSeq* contours = NULL;
	//CvMemStorage* storage = cvCreateMemStorage(0);
	// ����ԴͼƬ��覴ÿ����ø�ʴ�����������  
	//int count = cvFindContours(img_gray, storage, &contours, sizeof(CvContour), CV_RETR_EXTERNAL);
	//printf("����������%d", count);
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

	//	cvShowImage(szName, imgNo); //������и������ͼ����������򣬻���ϵ��£����ԱȽ�rc.x,rc.y;  
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