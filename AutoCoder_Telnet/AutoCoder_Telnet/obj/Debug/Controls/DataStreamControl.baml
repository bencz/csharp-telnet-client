   M S B A M L   `   `   `  ����K  GAutoCoder_Telnet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:AutoCoder.Telnet.ControlsAutoCoder.Telnet.Controls  5      W SAutoCoder, Version=1.0.7074.18330, Culture=neutral, PublicKeyToken=f834fd727b4fa2b1J3clr-namespace:AutoCoder.Controls;assembly=AutoCoderAutoCoder.Controls 5      1    +AutoCoder.Telnet.Controls.DataStreamControl   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      9local'clr-namespace:AutoCoder.Telnet.Controls     5      Bac3clr-namespace:AutoCoder.Controls;assembly=AutoCoder     5      .��5
      � -   5          �Name$  
LayoutRoot��5
       � RowDefinitions 5      �� 5   
   $
��auto �6   6(   �� 5   
   $��* �6   6%   �� 5   
   $
��auto �6   6(   �� 5      $
��auto �6   6&   .+�5      ]� $��0��6   ��. .+�5   
   �� $��5q�6   $��14(�6   $
��Bold"�6-   $
��Left=�6?   Description:6c   �� 5   
   $��5q�6   $��14(�6   ��6+   �� *DataStreamName+ �� UpdateSourceTrigger$ PropertyChanged=�6q   5   	   �� 5      $��1��6    �� SelectedIndex 6   �� *TabSelectedIndex+.@�5      �� $��Entry��6   .��5   
   �  5      �� 5      $��* �6   6)   �� 5      $
��auto �6   6,   �� 5      $
��auto �6   6,   .+�5      �� -   5        $��Lucida console)�5      $��14(�60   $��0��5      $��5q�5!      " �� VerticalScrollBarVisibility$ Visible=�5"       �� TextWrapping$ NoWrap=�5#       �� AcceptsReturn . ��5       �� *	ParseText+$ PropertyChanged=�5#   A   �� 5$      $��1��6    �� Orientation$ 
Horizontal=�6   .+�5%      �� -   5         ��Name$ butClear��5%      $��5q�6!   $��3q�6,   .��6O   Clear6V   �� 5&      -   5        $ 
butPrepare��5&      $��5q�6)   $��3q�64   .��6Y   	Prepare6b   5*      �� 5+      -   5        	 ��Name$	 tbErrmsg��5+      $��2��6"   61   5,      5-   	   �� 5.      $��Results��6   .��5/   
   (  "AutoCoder.Controls.EnhancedListBox  
 �� SelectionMode$
 Extended=�6   $��Lucida console)�67   ��6S   �� *ResultsList+6y   50   	   51      53      54      