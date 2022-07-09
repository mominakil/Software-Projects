
import java.util.ArrayList;


/**
 * This interface implements the abstract method used to display
 * objects onto the client or server UIs.
 */
public interface ChatIF 
{
  /**
   * Method that when overriden is used to display objects onto
   * a UI.
   */
  public abstract void display(String message);
  
  public abstract void displayUserList(ArrayList<String> userList, String roomName);
}
