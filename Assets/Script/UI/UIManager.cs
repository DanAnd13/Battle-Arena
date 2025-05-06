using BattleArena.Parameters;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BattleArena.Loader;

namespace BattleArena.UI
{
    public class UIManager : MonoBehaviour
    {
        public GameObject LobbyWindow;
        public GameObject SpawnWindow;
        public TextMeshProUGUI WeaponDropdawn;
        public TextMeshProUGUI ItemDropdawn;
        //public GameObject PlayButton;

        private GameBootstrapper _bootstrapper;

        private void Awake()
        {
            _bootstrapper = GetComponent<GameBootstrapper>();
        }

        void Update()
        {
            
                if (_bootstrapper.IsPalyerLoading)
                {
                    LobbyWindow.SetActive(true);
                }
                else
                {
                    LobbyWindow.SetActive(false);
                }
                if (_bootstrapper.IsPlayerJoin)
                {
                    SpawnWindow.SetActive(true);
                }
                else
                {
                    SpawnWindow.SetActive(false);
                }
                _bootstrapper.enteredWeponName = WeaponDropdawn.text;
                _bootstrapper.enteredItem = ItemDropdawn.text;
            }
    }
}
