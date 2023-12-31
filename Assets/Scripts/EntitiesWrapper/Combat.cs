using Amegakure.Starkane.Entities;
using Amegakure.Starkane.EntitiesWrapper;
using Amegakure.Starkane.GridSystem;
using Amegakure.Starkane.PubSub;
using bottlenoselabs.C2CS.Runtime;
using Dojo.Starknet;
using dojo_bindings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Character = Amegakure.Starkane.EntitiesWrapper.Character;

public class Combat : MonoBehaviour
{
    private MatchState matchState;
    private Dictionary<Player, List<Character>> playerMatchCharacters = new();
    private Dictionary<Player, List<ActionState>> actionStates = new();
    private Dictionary<Player, List<CharacterState>> characterStates = new();
    private DojoTxConfig dojoTxConfig;

    public MatchState MatchState
    {
        get => matchState;
        set
        {
            matchState = value;
            matchState.playerTurnIdChanged += MatchState_playerTurnIdChanged;
            matchState.winnerChanged += MatchState_winnerChanged;
        }
    }

    public Dictionary<Player, List<Character>> PlayerMatchCharacter { get => playerMatchCharacters; private set => playerMatchCharacters = value; }
    public Dictionary<Player, List<ActionState>> ActionStates { get => actionStates; private set => actionStates = value; }
    public Dictionary<Player, List<CharacterState>> CharacterStates { get => characterStates; private set => characterStates = value; }

    private void Awake()
    {
        playerMatchCharacters = new();
        actionStates = new();
        characterStates = new();
        dojoTxConfig = GameObject.FindAnyObjectByType<DojoTxConfig>();
    }

    private void Start()
    {
        BigInteger playerId = matchState.PlayerTurnId;
        // Debug.Log("Match turn playerID:" + playerId);
        // foreach (Player player in playerMatchCharacters.Keys)
        // {
        //     Debug.Log("ID in dict: " + player.Id);
        // }
        Player playerTurn = playerMatchCharacters.Keys.First(p => p.Id.Equals(playerId));

        EventManager.Instance.Publish(GameEvent.COMBAT_TURN_CHANGED, new() { { "Player", playerTurn } });
    }


    private void OnDisable()
    {
        matchState.playerTurnIdChanged -= MatchState_playerTurnIdChanged;
        matchState.winnerChanged -= MatchState_winnerChanged;
    }

    private void MatchState_playerTurnIdChanged(BigInteger playerId)
    {
        Player playerTurn = playerMatchCharacters.Keys.First(p => p.Id == playerId);

        EventManager.Instance.Publish(GameEvent.COMBAT_TURN_CHANGED, new() { { "Player", playerTurn } });
    }


    private void MatchState_winnerChanged(BigInteger playerId)
    {
        Player playerWinner = playerMatchCharacters.Keys.First(p => p.Id == playerId);
        Debug.Log("!!!!The Winnerr ....... is: " + playerWinner.PlayerName);

        EventManager.Instance.Publish(GameEvent.COMBAT_VICTORY, new() { { "Player", playerWinner } });
    }


    public void AddCharacter(Player player, Character character, ActionState actionState, CharacterState characterState)
    {
        if (!playerMatchCharacters.ContainsKey(player))
            playerMatchCharacters[player] = new();

        List<Character> currentCharacters = playerMatchCharacters[player];
        currentCharacters.Add(character);

        if (!actionStates.ContainsKey(player))
            actionStates[player] = new();

        List<ActionState> currentActionStates = actionStates[player];
        currentActionStates.Add(actionState);

        if (!characterStates.ContainsKey(player))
            characterStates[player] = new();

        List<CharacterState> currentCharacterStates = characterStates[player];
        currentCharacterStates.Add(characterState);
        characterState.OnDead += HandleOnDead;

        if (!character.IsAlive())
            EventManager.Instance.Publish(GameEvent.COMBAT_CHARACTER_DEAD, new() { { "Character", character } });
        // character.ResetLocation();
    }

    private void HandleOnDead(CharacterState state)
    {
        Player player = GetPlayerByID(state.Player_id);
        if(player != null)
        {
            Character character = playerMatchCharacters[player].Find(pmc => pmc.CharacterState.GetInstanceID() == state.GetInstanceID());;
            if(character != null)
                EventManager.Instance.Publish(GameEvent.COMBAT_CHARACTER_DEAD, new() { { "Character", character } });
        }
    }

    public void Move(Character character, Player player, Tile target)
    {
        if (CanMove(character, player))
        {
            CallMoveTx(character, player, target);
            character.Move(target);
        }
    }

    private void CallMoveTx(Character character, Player player, Tile target)
    {
        // Debug.Log("Move: " + player.PlayerName + "With: " + character.CharacterName
        //             + "To: " + target.coordinate.ToString());
        
        
        var provider = new JsonRpcClient(dojoTxConfig.RpcUrl);
        var account = new Account(provider, dojoTxConfig.GetKatanaPrivateKey(), dojoTxConfig.KatanaAccounAddress);

        string match_id_string = matchState.Id.ToString("X");
        var match_id = dojo.felt_from_hex_be(new CString(match_id_string)).ok;

        string player_id_string = player.Id.ToString("X");
        var player_id = dojo.felt_from_hex_be(new CString(player_id_string)).ok;

        string character_id_string = character.GetId().ToString("X");
        var character_id = dojo.felt_from_hex_be(new CString(character_id_string)).ok;

        string x_string = target.coordinate.x.ToString("X");
        var x = dojo.felt_from_hex_be(new CString(x_string)).ok;

        string y_string = target.coordinate.y.ToString("X");
        var y = dojo.felt_from_hex_be(new CString(y_string)).ok;

        dojo.Call call = new dojo.Call()
        {
            calldata = new[]
            {
                   match_id, player_id, character_id, x, y
                },
            to = dojoTxConfig.MoveSystemActionAddress,
            selector = "move"
        };

        account.ExecuteRaw(new[] { call });
    }

    public void CallEndTurnTX(Player player)
    {
        //TODO: check if player == playerTurn
        var provider = new JsonRpcClient(dojoTxConfig.RpcUrl);
        var account = new Account(provider, dojoTxConfig.GetKatanaPrivateKey(), dojoTxConfig.KatanaAccounAddress);

        List<dojo.FieldElement> calldata = new List<dojo.FieldElement>();

        string match_id_string = matchState.Id.ToString("X");
        var match_id = dojo.felt_from_hex_be(new CString(match_id_string)).ok;

        string player_id_string = player.Id.ToString("X");
        var player_id = dojo.felt_from_hex_be(new CString(player_id_string)).ok;

        dojo.Call call = new dojo.Call()
        {
            calldata = new[]
            {
                   match_id, player_id
                },
            to = dojoTxConfig.TurnSystemActionAddress,
            selector = "end_turn"
        };

        account.ExecuteRaw(new[] { call });
    }

    public bool CanMove(Character character, Player player)
    {
        bool playerRegistered = playerMatchCharacters.ContainsKey(player) && playerMatchCharacters[player].Contains(character);

        if (playerRegistered)
        {
            bool characterMoved = GetActionState(player, character).Movement;
            return matchState.PlayerTurnId == player.Id && !characterMoved;
        }

        return false;
    }

    private ActionState GetActionState(Player player, Character character)
    {
        return actionStates[player].Find(action => (int)action.Character_id == character.GetId());
    }

    public List<Character> GetCharacters(Player player)
    {
        return playerMatchCharacters[player];
    }

    public Player GetActualTurnPlayer()
    {
        BigInteger playerID = matchState.PlayerTurnId;
        foreach (Player player in playerMatchCharacters.Keys)
        {
            if (player.Id.Equals(playerID))
            {
                return player;
            }
        }

        return null;
    }

    public Player GetPlayerByID(BigInteger id)
    {
        BigInteger playerID = matchState.PlayerTurnId;
        foreach (Player player in playerMatchCharacters.Keys)
        {
            if (player.Id.Equals(id))
            {
                return player;
            }
        }

        return null;
    }

    public void DoSkill(Player playerFrom, Character characterFrom,
                            Skill skill, Player playerReceiver, Character characterReceiver)
    {
        if (CanDoSkill(playerFrom, characterFrom, skill) && characterReceiver.IsAlive())
        {
            CallSkillTX(playerFrom, characterFrom, skill, playerReceiver, characterReceiver);

            // Debug.Log("Skill");
            Dictionary<string, object> context = new()
            {
                { "PlayerFrom", playerFrom },
                { "CharacterFrom", characterFrom },
                { "Skill", skill },
                { "PlayerReceiver", playerReceiver },
                { "CharacterReceiver", characterReceiver }
            };

            EventManager.Instance.Publish(GameEvent.COMBAT_SKILL_DONE, context);

            if (!characterReceiver.IsAlive())
                EventManager.Instance.Publish(GameEvent.COMBAT_CHARACTER_DEAD, new() { { "Character", characterReceiver } });
        }
    }

    private void CallSkillTX(Player playerFrom, Character characterFrom,
                            Skill skill, Player playerReceiver, Character characterReceiver)
    {
        var provider = new JsonRpcClient(dojoTxConfig.RpcUrl);
        var account = new Account(provider, dojoTxConfig.GetKatanaPrivateKey(), dojoTxConfig.KatanaAccounAddress);

        List<dojo.FieldElement> calldata = new List<dojo.FieldElement>();

        string match_id_string = matchState.Id.ToString("X");
        var match_id = dojo.felt_from_hex_be(new CString(match_id_string)).ok;

        string player_id_string = playerFrom.Id.ToString("X");
        var player_id_from = dojo.felt_from_hex_be(new CString(player_id_string)).ok;

        string character_id_from_string = characterFrom.GetId().ToString("X");
        var character_id_from = dojo.felt_from_hex_be(new CString(character_id_from_string)).ok;

        string skill_id_string = skill.Id.ToString("X");
        var skill_id = dojo.felt_from_hex_be(new CString(skill_id_string)).ok;

        string level_string = skill.Level.ToString("X");
        var level = dojo.felt_from_hex_be(new CString(level_string)).ok;

        string player_id_receiver_string = playerReceiver.Id.ToString("X");
        var player_id_receiver = dojo.felt_from_hex_be(new CString(player_id_receiver_string)).ok;

        string character_id_receiver_string = characterReceiver.GetId().ToString("X");
        var character_id_receiver = dojo.felt_from_hex_be(new CString(character_id_receiver_string)).ok;

        dojo.Call call = new dojo.Call()
        {
            calldata = new[]
            {
                   match_id, player_id_from, character_id_from, skill_id, level,
                   player_id_receiver, character_id_receiver
                },
            to = dojoTxConfig.ActionSystemActionAddress,
            selector = "action"
        };

        account.ExecuteRaw(new[] { call });
    }

    public bool CanDoSkill(Player player, Character character, Skill skillSelected)
    {
        bool playerRegistered = playerMatchCharacters.ContainsKey(player) && playerMatchCharacters[player].Contains(character);

        if (playerRegistered)
        {
            // Debug.Log("Player name: " + player.PlayerName);
            ActionState playerActionState = GetActionState(player, character);
            CharacterState playerCharacterState = GetCharacterState(player, character);
            // Debug.Log("Character using: " + character.CharacterName);

            bool skillPerformed = playerActionState.Action;
            bool enoughMana = playerCharacterState.Remain_mp >= skillSelected.Mp_cost;
            // Debug.Log(" skillPerformed " + skillPerformed);
            // Debug.Log("enoughMana " + enoughMana);

            return !skillPerformed && enoughMana;
        }

        return false;
    }

    private CharacterState GetCharacterState(Player player, Character character)
    {
        return characterStates[player].Find(characterState => (int)characterState.Character_id == character.GetId());
    }

    internal List<Character> GetRivalCharacters(Player player)
    {
        // TODO: Verify if p != player works correctly
        List<Player> rivals = playerMatchCharacters.Keys.Where(p => p != player).ToList();
        List<Character> rivalCharacters = new();

        rivals.ForEach(r => rivalCharacters.AddRange(GetCharacters(r)));

        return rivalCharacters;
    }
}
