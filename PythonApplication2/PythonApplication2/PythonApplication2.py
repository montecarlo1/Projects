def printme():
   #"��ӡ������ַ�������׼��ʾ�豸��"
   x = 30
   print (x)
   return

printme()
#���ݿɱ����
def changeme(mylist):
  mylist.append([1,2,3])
  print("before changing", mylist)
  return

mylist = [10,20,30]
changeme(mylist)
print(mylist)