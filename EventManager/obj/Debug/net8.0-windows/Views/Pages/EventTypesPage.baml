   M S B A M L   `   `   `  ����  EventManager-    'EventManager.Views.Pages.EventTypesPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      .��5      �    LightGrayBrush#��[  6   .+�5   
   �   ActionBarStyle#��[ 6   $	��Top=�67   .+�5	      ��   �� Orientation$  
Horizontal=�6   $��Center=�62   .+�5
      �� -   5         ��Name$ 
HomeButton��5
        HamburgerButton#��[ 6-   $	����5      6(   �� 5      $��Event Types��6   $��18(�6/   $
��Bold"�6=     
WhiteBrush#��[ 5      $��Center=�6D   $
��10,0q�6_   5      5      �� 5   
   $��0,60,0,0q�6   .��5      ��   MobileContainer#��[ 6   .+�5      �� -   5         ��Name$ EventTypesListView��5      
������ $��0q�5      $
��0,10q�5      ��5      �� 5      .R�5      �� #��[ 6%     DarkGrayBrush#��[ 5   $   $��0,0,0,1q�5   $    �� Padding$ 15,10q�5   $   $	��0,2q�5    $   ._�5"   "   �� $  
Horizontal=�6-   .+�5$   &   �� $��⚙️��60   $��20(�6:   $��40��6H   $��Center=�6S   �� 5'   &   $��16(�5(   /   $��SemiBold"�5)   /   $��Center=�5*   /   $	��120��5+   /   $
��10,0q�5,   /   ��5'   0   �� *Name+5,   /   �� 5/   &   $  
Horizontal=�61   $��Right=�6J   .+�50   *   �� -   5        $��✏️��50   1   $��16(�51   0   
������ $��0q�53   0   $��30��54   0   $��30��55   0     PrimaryBlueBrush#��[ 57   0   �� 59   *   -   5        $��ℹ️��59   1   $��16(�5:   0   
������ $��0q�5<   0   $��30��5=   0   $��30��5>   0     SuccessGreenBrush#��[ 5@   0   �� 5B   *   -   5        $��🗑️��5B   1   $��16(�5C   0   
������ $��0q�5E   0   $��30��5F   0   $��30��5G   0     DangerRedBrush#��[ 5I   0   5J   '   5K   #   5L      5M      5O      �� 5R      #��[ 6   #��[ 5S      $��1q�5T       �� CornerRadius$ 8��5U      $ 15q�5V      $
��0,20q�5W      ._�5Y      �� .+�5Z      �� $��Add New Event Type��6$   $��18(�6>   $
��Bold"�6L   $��0,0,0,15q�6^   �� 5\      $��Event Type ID:��6$    	 
FieldLabel#��[	 6:   �� 5]      -   5         ��Name$ EventTypeIdTextBox��5]   "    
 LargeTextBox#��[
 6>   �� 5_      $��Name:��6$   #��[	 61   �� 5`      -   5        $ NameTextBox��5`   "   #��[
 67   �� 5b      $��Description:��6$   #��[	 68   �� 5c      -   5        $ DescriptionTextBox��5c   "   #��[
 6>   �� 5e      $��Icon:��6$   #��[	 61   �� 5f      -	   5        $��📁 Choose icon��5f   !     LargeMobileButton#��[ 6:   $
��Left=�5g       6;   �� 5i      $  
Horizontal=�6%   $��Center=�6>   $
��0,20q�6[   .+�5j      �� -
   5        $��➕ Add New Event Type��5j   %     SuccessButton#��[ 6D   $��5q�5k   $   6/   �� 5l      -   5        $��
❌ Cancel��5l   %     DangerButton#��[ 68   $��5q�5m   $   6/   5n      5o      5p      5r      5s      5t      5u      