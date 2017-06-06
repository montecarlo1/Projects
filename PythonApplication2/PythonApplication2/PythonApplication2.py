def printme():
   #"打印传入的字符串到标准显示设备上"
   x = 30
   print (x)
   return

printme()
#传递可变对象
def changeme(mylist):
  mylist.append([1,2,3])
  print("before changing", mylist)
  return

mylist = [10,20,30]
changeme(mylist)
print(mylist)