   M S B A M L   `   `   `  ����M  Isound-mixer-server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:SoundMixerServerSoundMixerServer  5   	   !    SoundMixerServer.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:SoundMixerServer     5   	   /tb http://www.hardcodet.net/taskbar     5   	   -   5          �Name$  window��5   	    � Title$ 
MainWindow��5	   	   $	��404��6   $	��520��6)   $��Right=�5
   	    � WindowStyle$
 None=�5   	    � AllowsTransparency . 
������  � 	Resources 5         TrayContextMenuShow �� 	Placement  TrayContextMenuHide��%H  &	        &	 �     �� 5   
   $ 
MousePoint=�62   .@�5      m� -   5        $
��Exit��5      6%   m� 5      -   5        $
��Show��5      6%   5      �� 5   
   $ 
MousePoint=�62   .@�5      m� -   5        $
��Exit��5      6%   m� 5      -   5        $
��Hide��5      6%   5      5      .��5      � -   5         �Name$ Grid1��5      $��1,0,2,1q�6   
������  � RowDefinitions 5   
   �� 5      $
��auto �6   �� 5!      $
��auto �6   �� 5#      $��* �6    � ColumnDefinitions 5&   
   �� 5(      $
��353* �6   �� 5*      $
��153* �6   �� 5+      $
��200* �6   .+�5.   
   �� -   5        	 ��Name$	 BorderSettingsHeader��5.      $��3��6/   $��0q�6C   
 J� SnapsToDevicePixels
 .  �� CornerRadius$ 2,2,0,0��6r     BackgroundDark#��[ 6�   $��40��5/      $	��Top=�6!   $��0,0,0,0q�69    J� BitmapEffect 50      F� 51       F� Color$ Black��6)    F� 	Direction$	 320Q�67    F� ShadowDepth$ 2Q�6G    F� Softness$ 15Q�6W    F� Opacity$ 0.275Q�6e   ._�53      � .+�54      ��  �� Orientation$ 
Horizontal=�6   $
��Left=�66   .+�55      �� $6��0pack://application:,,,/Resources/ic_settings.png��6   $��7q�6V   6c   �� 56      $��Sound Mixer Remote��6   $��Center=�69    �� ClearTypeHint$ Enabled=�6T    � UseLayoutRounding .   TextDeselected#��[ 6�   
 . $��18.667(�6�   $��Yu Gothic UI Semibold)�6�   57      �� 58      $ 
Horizontal=�6   $��Right=�66   .+�59      �� -   5          NoButtonStyle#��[ 59      $��0,3,3,3q�6D   .��5:      �� $3��-pack://application:,,,/Resources/ic_close.png��6    5;      5?      5@      5A      �� 5E   
   #��[ 6   $��2,0,2,0q�6?   $��0��6Y   $��1��6i   $��3��6v   ._�5F   
   �   MainBackground#��[ 6   .+�5H      �� ���#F0F0F0$��10, 10, 10, 10q�6*    5I      F� 5J      $ Black��6-   $	 320Q�6;   $ 2Q�6K   $ 15Q�6[   $ 0.275Q�6i   ._�5L      �� -	   5        $��stackPanelSettings��5L   4   $ Vertical=�6   $��0,0,0,0q�6N   .+�5M      � .+�5N      �� $��Running��6    $
��Left=�62   6O   �� 5O      -
   5        $��	cbRunning��5O   #   $��Center=�64   $��0,0,10,0q�6O   $��Right=�6a   #��[ 6}     CheckBoxStyle1#��[ 6�   6�   5P      � 5Q      .+�5R      �� $ 
Horizontal=�6%   .+�5S      �� $��Password Protected��6$   $
��Left=�6A   6^   �� 5T      $
��Left=�6$   .��5U   "   �� -   5        .�5U   D   Set Password6R   5V      5W      �� 5X      -   5        $��cbUsesPassword��5X   #   $��Center=�69   $��0,0,10,0q�6T   $��Right=�6f   #��[ 6�   #��[ 6�   6�   5Y      � 5Z      -   5        $��gridEnterPassword��5Z      $y�	Collapsed=�64   .+�5[      �� $��30,0,0,0q�6    $��Password��62   $
��Left=�6E   $��Center=�6`   6}   �� 5\      $ 
Horizontal=�6%   $��Right=�6>   .+�5]      �� -   5        $��
tbPassword��5]   &     MaterialTextBox#��[ 68   $	��100��6a   $��0,0,10,0q�6m   6�   �� 5^      -   5        $	��Set��5^   %     RoundedButtonSecondary#��[ 63   $��50��6x   $��0,0,10,0q�6�   6�   5_      5`      � 5a      .+�5b      �� $��Start with Windows��6    $
��Left=�6=   6Z   �� 5c      -   5        $��cbAutostart��5c   #   $��Center=�66   $��0,0,10,0q�6Q   $��Right=�6c   #��[ 6   #��[ 6�   6�   5d      5f      5g      5h      5i      �� 5l   
   #��[ 6   $��2,0,2,5q�6?   $��2��6Y   $��3��6f   ._�5m      � #��[ 6   .+�5p      � $��0��6   .+�5q      �� -   5        $	 brdDevicesHeader��5q      
 . $��0q�6R   $ 2,2,0,0��6f   #��[ 6}   $��40��6�   $	��Top=�6�   $��
10,0,10,10q�6�    5r      F� 5s      $ Black��65   $	 320Q�6C   $ 2Q�6S   $ 15Q�6c   $ 0.275Q�6q   ._�5u      � .+�5v      �� $ 
Horizontal=�6)   .+�5w   "   �� $3��-pack://application:,,,/Resources/ic_phone.png��6(   $��7q�6_   
 . 6�   �� 5x   "   $��Devices��6(   $��Center=�6:   
 . #��[ 6p   $��18.667(�6�   $��Yu Gothic UI Semibold)�6�   5y      5z      5{      �� 5|      $ Vertical=�6!   $��10,40,10,10q�68   #��[ 6M    5}      F� 5~      $ Black��65   $	 320Q�6C   $ 2Q�6S   $ 15Q�6c   $ 0.275Q�6q   .+�5�      &   System.Windows.Controls.DataGrid  -   5        $��	dgDevices��5�   #   ��.  
 .    AutoGenerateColumns .     HorizontalGridLinesBrush
 ���� $��0q�6�    	 BackgroundMedium   VerticalGridLinesBrush# [	 5�   +    
 BackgroundLight   AlternatingRowBackground# [
 6f    	 White   RowBackground# [ 6�     ColumnHeaderHome   ColumnHeaderStyle# [ 5�   +      CanUserAddRows .     CanUserResizeRows .     CanUserResizeColumns .     CanUserSortColumns .    
IsReadOnly . $��Yu Gothic UI Semibold)�5�   =   $��13.333(�6`       HeadersVisibility$  Column=�5�   +   ��6F   �� !   Columns! 5�      0  *System.Windows.Controls.DataGridTextColumn  5�      ,  &System.Windows.Controls.DataGridColumn"   Width5  /System.Windows.Controls.DataGridLengthConverter$" * 61   #   Header$
# Name��6;   $   FontSize$$ 12Q�6I   1  +System.Windows.Controls.DataGridBoundColumn%   Binding% 6W   �� *Name+6r     5�      $" * 61   $# 
IP-Address��6;   $$ 12Q�6O   % 6]   �� *IP+6v     5�      $" * 61   $# 	Connected��6;   $$ 12Q�6N   % 6\   �� *ConnectedString+6�     5�      $
" 1.5* 61   $# Last Connected��6>   $$ 12Q�6V   % 6d   �� *LastConnectedString+6�   5�        5�      -   5        $��	dgPadding��5�   #   ��.  
 .  .  
 ���� $��0q�6�   # [	 5�   +   # [
 6f   # [ 5�   +    .   .   .   .  . $��Yu Gothic UI Semibold)�5�   =   $��13.333(�6`   $
  None=�5�   +   ��6D   �� ! 5�        5�   "   $" * 65   $
# Name��6?   $$ 12Q�6M   % 6[   �� *Name+6v     5�   "   $" * 65   $# 
IP-Address��6?   $$ 12Q�6S   % 6a   �� *IP+6z     5�   "   $" * 65   $# 	Connected��6?   $$ 12Q�6R   % 6`   �� *ConnectedString+6�     5�   "   $
" 1.5* 65   $# Last Connected��6B   $$ 12Q�6Z   % 6h   �� *LastConnectedString+6�   5�      5�      5�      5�      5�      5�      5�      