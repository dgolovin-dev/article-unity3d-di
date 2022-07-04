using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace context.example {
    public class HpPanel : SceneContextMonoBehaviour {
        [SerializeField]
        private Text label;
        [Inject][NotEditable]
        private Starship starship;

        [AfterInject]
        private void AfterInject() {
            starship = GameObject.FindGameObjectWithTag("starship").GetComponent<Starship>();
            starship.onHpChanged.AddListener(UpdateLabel);
            UpdateLabel();
        }

        public void UpdateLabel() {
            label.text = starship.hp.ToString();
        }
    }
}
