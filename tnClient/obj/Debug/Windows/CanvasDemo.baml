   M S B A M L   `   `   `  ����C  ?tnClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:tnClient.WindowstnClient.Windows  5   	   !    tnClient.Windows.CanvasDemo   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:tnClient.Windows     5   	     � Title$  
CanvasDemo��5   	   $	��300��6   $	��300��6)   .��5	      �  � RowDefinitions 5
      �� 5      $
��auto �6    60   �� 5      $��* �6    6-   �� 5      $
��auto �6    60    � ColumnDefinitions 5      �� 5      $��* �6#   6/   .+�5      o� -   5        $��menu1��5      $
��Left=�5      $��0��6/   $	��Top=�5      .@�5      m� -   5        $
��Test��5      6B   m� 5      -   5        $��Focus��5      6?   m� 5      -   5        $
��Exit��5      6>   5      �� 5   
   -   5         ��Name$ TabControl1��5      $��1��6*   $��0��67   .@�5      �� $��Log file��6   .��5      �� -   5         ��Name$ lbLog��5      6+   5      �� 5      -   5         ��Name$ TabItem1��5      $��Canvas��6(   .��5      �� -   5         ��Name$ Canvas1��5      $��1��6*   
����  �|�. .+�5"      �� -	   5         ��Name$ 
TextBlock1��5"       $	��abc��64   $��18(�6?   
�������6b   5#      5$      5%      5'      5(      