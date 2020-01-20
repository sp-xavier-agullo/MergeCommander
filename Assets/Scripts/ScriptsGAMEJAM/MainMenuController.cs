using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GJ18
{
    public class MainMenuController : MonoBehaviour
    {
        public LevelConfig[] _levelConfigs;

        public void OnClickedPlay()
        {
            GameplayController.PlayLevelConfig = _levelConfigs[MenuManager.GetCurrentLevel() - 1];
            SceneManager.LoadScene("Gameplay");
        }
    }
}