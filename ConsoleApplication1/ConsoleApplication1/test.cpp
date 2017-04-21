#include <windows.h>
#include <stdio.h>
#include <conio.h>
#include <iostream>
#include <fstream>

typedef unsigned long ulong;
using namespace std;
char before[11][11] = { 0 };
char after[11][11] = { 0 };
int n = 0;
ulong result;
ulong prev_result;
ulong next_result;
int b[] = { 9,8,7,5,6,4,3,0,1,2 };
int length = sizeof(b) / sizeof(*b);
void bubble_sort(int a[])
{
	for (int i = 0; i < length; i++)
	{
		for (int j = 0;  j < length-i-1; j++)
		{
			if (a[j] > a[j + 1])
			{
				int temp = a[j];
				a[j] = a[j+1];
				a[j + 1] = temp;
			}
		}
	}
}
void insert_sort(int a[])
{
	int i, j, tmp;
	for (i = 1; i < length; i++)
	{
		//int tmp;
		tmp = a[i];
		for (j = i-1; j >= 0 && a[j] > tmp; j--)
		{
			a[j + 1] = a[j];
		}
		a[j + 1] = tmp;
	}
}
void select_sort(int a[])
{
	for (int i = 0; i < length; i++)
	{
		for (int j = i + 1; j < length; j++)
		{
			if (a[i] > a[j])
			{
				int temp = a[i];
				a[i] = a[j];
				a[j] = temp;
			}
		}
	}
}
void quick_sort(int a[], int left, int right)
{	
	if (left > right)
	{
		return;
	}
	int i = left;
	int j = right;
	int temp = a[left];
	while (i < j)
	{
		while (i < j && a[j] >= temp)
		{
			j--;
		};
		a[i] = a[j];
		while (i < j && a[i] <= temp)
		{
			i++;
		}
		a[j] = a[i];
	}
	a[i] = temp;
	quick_sort(a, left, i - 1);
	quick_sort(a, i + 1, right);
}
ulong Fib(int n) 
{
	result = prev_result = 1;
	while (n > 2)
	{ 
		next_result = prev_result;
		prev_result = result;
		result = next_result + prev_result;
		n--;
	}
	return result;
}
//-1~-3 Rotation.
bool option1()
{
	std::cout << "Herfwerw" << std::endl;
	for (int i = 1; i <= n; i++)
		for (int j = 1; j <= n; j++)
			if (before[i][j] != after[j][n-i+1])
				return false;
	return true;
}
bool option2()
{
	for (int i = 1; i <= n; i++)
		for (int j = 1; j <= n; j++)
			if (before[i][j] != after[n-i+1][n-j+1])
				return false;
	return true;
}
bool option3()
{
	for (int i = 1; i <= n; i++)
		for (int j = 1; j <= n; j++)
			if (before[i][j] != after[n - j + 1][i])
				return false;
	return true;
}
//Symmetry exchange.
bool option4()
{
	for (int i = 1; i <= n; i++)
		for (int j = 1; j <= n; j++)
			if (before[i][j] != after[i][n - j + 1])
				return false;
	return true;
}
//Combination.
bool option5()
{
	char temp[11][11] = {0};    
	for (int i = 1; i <= n; i++)
		for (int j = 1; j <= n; j++)
			temp[i][j] = before[i][j];
	for (int i = 1; i <= n; i++)
		for (int j = 1; j <= n; j++)
			before[i][n-j+1] = temp[i][j];
	if (option1 || option2 || option3)
	{
		return true;
	}
	else
	{
		return false;
	}
}
//No changes.
bool option6()
{
	for(int i = 1; i <= n; i++)
		for (int j = 1; j <= n; j++)
			if (before[i][j] != after[i][j])
				return false;
    return true;     
}
int main()
{
	int m;
	cout << "please input a number:" << endl;
	cin >> m;
	cout << "result:" << endl;
	//cout << Fib(m) << endl;
	//insert_sort(b);
	//bubble_sort(b);
	//select_sort(b);
	//quick_sort(b, 0, length-1);/*这里原文第三个参数要减1否则内存越界*/
	/*for (int k = 0; k < length; k++)
	{
		cout << b[k] << " ";
	}
	cout << endl;*/



	ifstream fin("marks.txt");
	fin >> n;
	for (int i = 1; i <= n; i++)
		for (int j = 1; j <= n; j++)
			fin >> before[i][j];
	for (int i = 1; i <= n; i++)
		for (int j = 1; j <= n; j++)
			fin >> after[i][j]; 
	/*for (int i = 1; i <= n; i++)
	{
		for (int j = 1; j <= n; j++)
		{
			after[i][n - j + 1] = before[i][j];
		}			
	}

	for (int i = 1; i <= n; i++)
	{
		for (int j = 1; j <= n; j++)
		{
			cout << after[i][j];
		}
		cout << endl;
	}*/

	if (option1())
	{
		cout << 1 << endl;
		return 0;
	}
	else if (option2())
	{
		cout << 2 << endl;
		return 0;
	}
	else if (option3())
	{
		cout << 3 << endl;
		return 0;
	}
	else if (option4())
	{
		cout << 4 << endl;
		return 0;
	}
	else if (option5())
	{
		cout << 5 << endl;
		return 0;
	}
	else if (option6())
	{
		cout << 6 << endl;
		return 0;
	}
	else
	{
		cout << 7 << endl;
		return 0;
	}	
	return 0;
}