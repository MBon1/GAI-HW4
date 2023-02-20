using UnityEngine;
using UnityEngine.SceneManagement;

public class Init : MonoBehaviour
{
    public void GoToSim()
    {
        SceneManager.LoadScene("MovementTest");
    }
    public void GoToObstacleSim()
    {
        SceneManager.LoadScene("ObstacleTest");
    }
    public void GoToScalableSim()
    {
        SceneManager.LoadScene("ScalableFormationTest");
    }
    public void GoTo2LevelSim()
    {
        SceneManager.LoadScene("New2LevelTest");
    }
}
