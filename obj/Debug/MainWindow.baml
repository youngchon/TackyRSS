   M S B A M L   `   `   `  ����d  `Microsoft.Maps.MapControl.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35tRclr-namespace:Microsoft.Maps.MapControl.WPF;assembly=Microsoft.Maps.MapControl.WPFMicrosoft.Maps.MapControl.WPF  5   	    TackyRSS   TackyRSS.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   abmRclr-namespace:Microsoft.Maps.MapControl.WPF;assembly=Microsoft.Maps.MapControl.WPF     5   	   -   5          �Name$  tackrss��5   	    � Title$ 
MainWindow��5   	   $	��800��6   $
��1200��6)   .��5      � .+�5	   
   o� $��0,0,-0.4,737q�6   .@�5
      m� $��_File��6   .@�5      m� -   5         m�Name$ 	menuLogin��5      $��_Login��6.   6>   m� 5      -   5        $ menuLougout��5      $��	_Sign Out��60   6C   m� 5      -   5        $ menuExit��5      $��_Exit��6-   6<   5      m� 5      $��_Channel��6   .@�5      m� -   5        $ menuChannelAdd��5      $��_Add A Channel��63   6K   m� 5      -   5        $ menuChannelRemove��5      $��_Remove Selected Channel��66   6X   m� 5      -   5        $ menuChannelUpdate��5      $ ��_Update Selected Frequency��66   6Z   5      m� 5      $��_Article��6   .@�5      m� -   5        $ menuStoryMap��5      $��_Pin Article!��61   6H   m� 5      -	   5        $ menuStoryUnread��5      $��_Mark Story As Unread��64   6S   5      5      �� 5   
   -
   5         ��Name$ ChannelInformationBox��5      $��Center=�61   $��50,318,910.6,0q�6N   $	��Top=�6f   $	��233��6~   $��90��6�   �� 5   
   -   5        $ StoryTitlesBox��5      $
��Left=�6*   $	��218��6E   $��
50,413,0,0q�6R   $	��Top=�6f   $	��233��6~   6�   �� 5   
   -   5         ��Name$ StoryDescriptionBox��5      $
��Left=�61   $��
359,64,0,0q�6L   $
��Wrap=�6`   $�� ��6t   $	��Top=�6|   $��73��6�   $	��800��6�   �� 5   
   -   5        $ 
ChannelBox��5      $
��Left=�6&   $	��240��6A   $��	50,77,0,0q�6N   $	��Top=�6a   $	��233��6y   6�   � 5   
   -   5         �Name$ loadProgressBar��5      $
��Left=�6/   $��10��6J   $��
10,752,0,0q�6V   $	��Top=�6j   $	��100��6�   �� 5   
   -   5         ��Name$ wintab��5      $
��Left=�6%   $	��600��6@   $��359,153,0,0q�6M   $	��Top=�6b   $	��800��6z    �� SelectedIndex$ 0��6�   .@�5       �� -   5         ��Name$ bmmap��5       $	��Map��6%   .��5!      '   !Microsoft.Maps.MapControl.WPF.Map  -   5        	  Name$	 tackyMap��5!      +   %Microsoft.Maps.MapControl.WPF.MapCore
   CredentialsProviderT   NMicrosoft.Maps.MapControl.WPF.Design.ApplicationIdCredentialsProviderConverter$F
 @AuWJ_GJP3aPOz_C92W_l2ClKwCcr-E6A-yF3m4TrpTiDQHkYBZ6-OnxmsJhsWlsC 6+      Center<   6Microsoft.Maps.MapControl.WPF.Design.LocationConverter$ 39.3683,-95.2734,0.0000 5"         	ZoomLevel$ 4.000Q�6>      AnimationLevel$
 None=�6P      Mode;   5Microsoft.Maps.MapControl.WPF.Design.MapModeConverter$ AerialWithLabels 6f   5#      �� 5$      -   5        $ wb��5$      $��Web Browser��6"   .��5%      (  "System.Windows.Controls.WebBrowser  -   5          Name$ tackyrssbrowser��5%      $
��Left=�66   $	��Top=�6Q   6i   5&      5'      5(      5)      