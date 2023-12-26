using Amegakure.Starkane.GridSystem;
using Amegakure.Starkane.PubSub;
using System.Collections.Generic;
using System;
using UnityEngine;
using Amegakure.Starkane.EntitiesWrapper;

namespace Amegakure.Starkane.UI.Spatial
{
    [RequireComponent(typeof(TileRenderer))]
    public class FrontierIllustrator : MonoBehaviour
    {
        private TileRenderer tileRenderer;

        private void Awake()
        {
            tileRenderer = GetComponent<TileRenderer>();
        }

        private void OnEnable()
        {
            EventManager.Instance.Subscribe(GameEvent.PATH_FRONTIERS_RESET, HandlePathFrontiersReset);

            EventManager.Instance.Subscribe(GameEvent.INPUT_CHARACTER_SELECTED, HandleCharacterSelected);
            EventManager.Instance.Subscribe(GameEvent.INPUT_CHARACTER_UNSELECTED, HandleCharacterUnselected);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe(GameEvent.PATH_FRONTIERS_RESET, HandlePathFrontiersReset);
            EventManager.Instance.Unsubscribe(GameEvent.INPUT_CHARACTER_SELECTED, HandleCharacterSelected);
            EventManager.Instance.Unsubscribe(GameEvent.INPUT_CHARACTER_UNSELECTED, HandleCharacterUnselected);
        }

        private void HandleCharacterUnselected(Dictionary<string, object> context)
        {
            try
            {
                Character character = (Character)context["Character"];

                Frontier frontierToClean = character.GetMovementFrontier();
                if(frontierToClean != null)
                    tileRenderer.ClearColor(frontierToClean.Tiles);
            }
            catch { }
        }

        private void HandleCharacterSelected(Dictionary<string, object> context)
        {
            try
            {
                Character character = (Character)context["Character"];
                Frontier frontier  = character.GetMovementFrontier();

                this.IllustrateFrontier(frontier);
            }
            catch { }
        }

        private void HandlePathFrontiersReset(Dictionary<string, object> context)
        {
            try
            {
                List<Tile> tiles = (List<Tile>)context["Tiles"];
                tileRenderer.ClearColor(tiles);
            }
            catch (Exception e) { Debug.LogError(e); }
        } 

        public void IllustrateFrontier(Frontier frontier)
        {
            tileRenderer.SetActiveTiles(frontier.Tiles);
        }
    }
}

