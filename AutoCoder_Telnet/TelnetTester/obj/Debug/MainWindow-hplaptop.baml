   M S B A M L   `   `   `  ����G  CTelnetTester, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:TelnetTesterTelnetTester  5   	       TelnetTester.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:TelnetTester     5   	     � Title$  
MainWindow��5   	   $	��350��6   $	��525��6)   .��5	      � -   5         �Name$ grdMain��5	       � RowDefinitions 5
   
   �� 5      $
��auto �6   6,   �� 5      $��* �6   6)   �� 5      $
��auto �6   6,   .+�5   
   o� -   5        $��menu1��5      $
��Left=�5      $��0��6+   $	��Top=�5      .@�5      m� -   5        $
��Test��5      6>   m� 5      -   5        $��Parse server stream��5      6M   m� 5      -   5        $��Parse response stream��5      6O   m� 5      -   5        $��Print��5      6?   m� 5      -   5        $��	Clear log��5      6=   m� 5      -   5        $
��Exit��5      68   5      �� 5   
   -	   5         ��Name$ TabControl1��5      $��1��6*   $��0��67   .@�5      �� $��Log file��6   .��5      �� -
   5         ��Name$ lbLog��5       �� SelectionMode$ Multiple=�6)   $��Lucida console)�5      $��14(�66   ��5      �� *RunLog+6;   5      �� 5!      $��Canvas��6   .��5"      �� -   5         ��Name$ Canvas1��5"      
����  �5#   5   5$      �� 5%      $��Data stream��6   .��5&      �  5'      �� 5(      $��* �6$   61   �� 5)      $
��auto �6$   64   .+�5+      �� -   5         ��Name$ TextBox1��5+      $��Lucida console)�6,   $��14(�6H   $��5q�5,      " �� VerticalScrollBarVisibility$ Visible=�5-      	 �� TextWrapping$	 NoWrap=�5.      
 �� AcceptsReturn
 . 6A   �� 5/      $��1��6!    �� Orientation$ 
Horizontal=�6.   .+�50      �� -   5         ��Name$ butClear��50   !   $��5q�63   $��3q�6>   .��6a   Clear6h   �� 51      -   5        $ butParse��51   !   $��5q�63   $��3q�6>   .��6a   Parse6h   �� 52      -   5        $ butParseResponse��52   !   $��5q�6;   $��3q�6F   .��6q   Parse response6�   53      55      56      �� 58      $��Settings��6   .��59      �  � 	Resources 5;       �� 
TargetType �� Property �� Value��%}   '��       �� 5<      " ��6    .�5=      ��  � ��$ 2,0,4,0q�67   �� 5>       � ��$
 Bold"�6;   5?      5@       5B      �� 5C      $
��Auto �6(   68   �� 5D      $
��Auto �6(   68   �� 5E      $
��Auto �6(   68    � ColumnDefinitions 5H      �� 5I      $
��Auto �6+   �� 5J      $��* �6+   .+�5M      �� $��0��6   .��6)   Connect to system6<   �� 5N      $��0��6    $��1��6-   $	��120��6=    �� 
IsEditable . 
���Gc�� �� SelectedItem 5P       �� *SystemListSelectedItem+ �� Text 5Q      �� *
SystemName+ �� Mode$ Default=� �� UpdateSourceTrigger$ 	LostFocus=���5R      �� *
SystemList+�� 5U      $��1��6   .��6)   Auto connect67   �� 5V      $��Center=�6!   $��1��5X      $��1��6,    d� 	IsChecked 5W      ��  �� Path$ AutoConnect�5Y      5Z      5[      5\      5^      5_      