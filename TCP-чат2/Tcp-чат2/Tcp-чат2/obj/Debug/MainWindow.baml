   M S B A M L   `   `   `  ????F  BTcp-чат2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:Tcp_чат2Tcp_чат2  5   	       Tcp_чат2.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Hi:http://schemas.microsoft.com/expression/2010/interactivity     5   	   Hei9http://schemas.microsoft.com/expression/2010/interactions     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:Tcp_чат2     5   	   -   5          ? Title$  Месседжер??5   	   $	??450??6   $	??800??6(    ? 
ResizeMode$ NoResize=?64   .??5      ? .+?5   
   ?? -   5        $??
SendButton??5      $??35??6#   $??590,374,10,12q?6/   .??6_   Отправить6j   ?? 5   
   -   5        $??SendText??5      $??10,374,209,10q?6#   6X   ?? 5   
   -   5        $??ChatArea??5      $??	10,79,0,0q?6"    ?? 
IsReadOnly . $
??Left=?6G   $	??575??6b   $	??290??6n   $	??Top=?6{   6?   ?? 5   
   -   5         ??Name$ 	userLabel??5      $??75,20,617,0q?6#   $??Пользователь??68   $	??Top=?6O   ?? 5   
   -   5        $??userName_Add??5       ?? TextAlignment$ Center=?6&    J? RenderTransformOrigin$ 0.702,0.799&?6=   $??181,26,312,0q?6a   $??20??6w   $	??Top=?6?   ?? 5   
   -   5        $??	chatEntry??5      $??45??6"   $	??100??6.   $??
487,26,0,0q?6:   $	??Top=?6f   $
??Left=?6~   .??6?   
Вход6?   ?? 5   
   -   5         ??Name$ UserList??5   +    ?? SelectionMode$ Multiple=?6   $
??Left=?6=   $	??290??6X   $??
590,79,0,0q?6e   $	??Top=?6y   $	??194??6?   ?? 5   
   -	   5         ??Name$ Host_Add??5      $ Center=?6$   $ 0.702,0.799&?6;   $??182,51,386,0q?6_   $??20??6u   $	??Top=?6?   ?? 5   
   -
   5        $ 	hostLabel??5      $??75,48,617,0q?6#   $??Хост и Порт??68   $ 0.843,0.269&?6N   $	??Top=?6r   ?? 5   
   -   5        $ Port_Add??5      $ Center=?6$   $ 0.702,0.799&?6;   $??413,51,313,0q?6_   $??20??6u   $	??Top=?6?   5      5      