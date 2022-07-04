using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace direct {
    public class HpPanel : MonoBehaviour {
        [SerializeField]
        private Text label;
        [SerializeField]
        private Starship starship;

        private void Start() {
            UpdateLabel();
        }

        public void UpdateLabel() {
            label.text = starship.hp.ToString();
        }
    }
}
