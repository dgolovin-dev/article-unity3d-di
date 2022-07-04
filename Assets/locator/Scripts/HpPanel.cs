using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace locator {
    public class HpPanel : MonoBehaviour {
        [SerializeField]
        private Text label;
        [SerializeField][NotEditable]
        private Starship starship;

        private void Start() {
            starship = GameObject.FindGameObjectWithTag("starship").GetComponent<Starship>();
            starship.onHpChanged.AddListener(UpdateLabel);
            UpdateLabel();
        }

        public void UpdateLabel() {
            label.text = starship.hp.ToString();
        }
    }
}
