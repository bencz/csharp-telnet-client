   M S B A M L   `   `   `  ����G  CTelnetTester, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:TelnetTesterTelnetTester  5   	   9!clr-namespace:TelnetTester.CommonTelnetTester.Common  5   	   K GAutoCoder_Telnet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null_Aclr-namespace:AutoCoder.Telnet.Controls;assembly=AutoCoder_TelnetAutoCoder.Telnet.Controls 5   	       TelnetTester.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:TelnetTester     5   	   1com!clr-namespace:TelnetTester.Common     5   	   PucAclr-namespace:AutoCoder.Telnet.Controls;assembly=AutoCoder_Telnet     5   	     � Title$  Telnet tester��5
   	   $	��350��6   $	��525��6,   .��5      � -   5         �Name$ grdMain��5       � RowDefinitions 5   
   �� 5      $
��auto �6   6,   �� 5      $��* �6   6)   �� 5      $
��auto �6   6,   .+�5   
   o� -   5        $��menu1��5      $
��Left=�5      $��0��6+   $	��Top=�5      .@�5      m� -   5        $
��Test��5      6>   m� 5      -   5        $��Parse server stream��5      6M   m� 5      -   5        $��Parse response stream��5      6O   m� 5      -   5        $��Print��5      6?   m� 5      -   5        $��	Clear log��5      6=   m� 5      -   5        $
��Exit��5      68   5      �� 5   
   -	   5         ��Name$ TabControl1��5      $��1��6*   $��0��67    �� SelectedIndex 5      �� *TabSelectedIndex+.@�5      �� $��Log file��6   .��5      $   TelnetTester.Common.NewListBox  -
   5          Name$ lbLog��5   !   $   TelnetTester.Common.NewListBox   PreviewKeyDown$ lbLog_PreviewKeyDown��5       �� SelectionMode$ Extended=�5   0   $��Lucida console)�5       $��14(�66   ��5!      �� *RunLog+6;   5"      �� 5$      $��Canvas��6   .��5%   
   �  5&      �� 5'      $
��auto �6   6,   �� 5(      $��* �6   6)   .+�5+      o� -   5         o�Name$ menu2��5,      $
��Left=�5+      $��0��6-   $	��Top=�5,       .@�5-      m� -   5        $��Report items��5-      6D   5.      �� 50      -   5        	 ��Name$	 Canvas1��50       $��1��6   
����  �51   5   52      53   	   �� 54      $��Data stream��6   .��55   
   � .+�56      1  +AutoCoder.Telnet.Controls.DataStreamControl  -   5        
  Name$
 DataStreamControl1��56   !      DataStreamName 57   !   �� *DataStreamName+   	ParseText 58   !   �� *	ParseText+59      5:   	   �� 5<      $��Settings��6   .��5=      �  � 	Resources 5?       �� 
TargetType �� Property �� Value��%}   '��       �� 5@      " ��6    .�5A      ��  � ��$ 2,0,4,0q�67   �� 5B       � ��$
 Bold"�6;   5C      5D       5F      �� 5G      $
��Auto �6(   68   �� 5H      $
��Auto �6(   68   �� 5I      $
��Auto �6(   68    � ColumnDefinitions 5L      �� 5M      $
��Auto �6+   �� 5N      $��* �6+   .+�5Q      �� $��0��6   .��6)   Connect to system6<   �� 5R      $��0��6    $��1��6-   $	��120��6=    �� 
IsEditable . 
���Gc�� �� SelectedItem 5T       �� *SystemListSelectedItem+ �� Text 5U      �� *
SystemName+ �� Mode$ Default=� �� UpdateSourceTrigger$ 	LostFocus=���5V      �� *
SystemList+�� 5Y      $��1��6   .��6)   Auto connect67   �� 5Z      $��Center=�6!   $��1��5\      $��1��6,    d� 	IsChecked 5[      ��  �� Path$ AutoConnect�5]      5^      5_      �� 5`      $��Notepad��6   .��5a   
   �  5b      �� 5c      $
��auto �6   6,   �� 5d      $��* �6   6)   .+�5g      ]� $��0��6   $��0��6#   ��. .+�5h      �� -   5         ��Name$ butEdit��5h      $��Right=�6&   $��3q�5i      $��3q�6    .��5j   +   Edit61   �� 5k      -   5        $ butTextFilePath��5k   ,   $��Right=�6   $	��...��5m      $��3q�5n      $��3q�6   �� 5o      $��80��64   $��3q�6B   ��6   �� *TextFilePath+6B   5p      �� 5r      -   5         ��Name$ RichTextBox1��5r      $��1��6.   $��10q�5s      &   System.Windows.Media.TextOptions   TextRenderingMode$ Aliased=�5t       �� 
AcceptsTab .  �� AcceptsReturn . " �� VerticalScrollBarVisibility$
 Auto=�5v      5w      5x      5y   	   5z      5|      5}      