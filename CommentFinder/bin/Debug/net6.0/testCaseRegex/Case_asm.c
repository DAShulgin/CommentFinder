#include <iostream.h> //подключаем функцию вывода текста на экран cout 
 
int main()
{
   long long a,b;
 
   asm{
     CPUID
     RDTSC
     mov DWORD PTR[a], eax
     mov DWORD PTR[a + 4], edx
   }
 
   for(int i = 0; i < 10; i++)
     cout << " go next\n";

 long long c = b - a;
  cout << c;
}