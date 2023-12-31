using System;
using System.Collections.Generic;
using bottlenoselabs.C2CS.Runtime;
using Dojo.Starknet;
using dojo_bindings;
using UnityEngine;

struct PlayerCharacter
{
    public dojo.FieldElement player;
    public dojo.FieldElement character_id;
}

public class SystemsCalls : MonoBehaviour
{

    private void Start()
    {
        var hash = new Hash128();
        hash.Append("enemy");
        hash.Append("enmy");
        string hashString = hash.ToString();
        Debug.Log(hashString);
    }
    private void mint()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            string rpcUrl = "http://localhost:5050";

            var provider = new JsonRpcClient(rpcUrl);
            var signer = new SigningKey("0x1800000000300000180000000000030000000000003006001800006600");
            string playerAddress = "0x517ececd29116499f4a1b64b094da79ba08dfd54a3edaa316134c41f8160973";

            var account = new Account(provider, signer, playerAddress);
            string actionsAddress = "0x57a6556e89380b76465e525c725d8ed065a03b47fb9a4c9b676a1afea8177c5";


            var hash = new Hash128();
            hash.Append("Santi");
            hash.Append("Hello");
            string hashString = hash.ToString();
            Debug.Log(hashString);

            var character_id = dojo.felt_from_hex_be(new CString("0x02")).ok;
            var player_id = dojo.felt_from_hex_be(new CString(hashString)).ok;

            Debug.Log(player_id);
            dojo.Call call = new dojo.Call()
            {
                calldata = new dojo.FieldElement[]
                {
                        character_id,
                        player_id,
                        dojo.felt_from_hex_be(new CString("0x01")).ok
                },
                to = actionsAddress,
                selector = "mint"
            };

            account.ExecuteRaw(new[] { call });
        }
    }

    private void combat()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            string rpcUrl = "http://localhost:5050";

            var provider = new JsonRpcClient(rpcUrl);
            var signer = new SigningKey("0x1800000000300000180000000000030000000000003006001800006600");
            string playerAddress = "0x517ececd29116499f4a1b64b094da79ba08dfd54a3edaa316134c41f8160973";

            var account = new Account(provider, signer, playerAddress);
            string actionsAddress = "0x2d4efd349d469a05680cb7f1186024b8d95c33bd11563de07fe687ddcbfa483";

            var character_id = dojo.felt_from_hex_be(new CString("0x03")).ok;
            var player_id = dojo.felt_from_hex_be(new CString("0x01")).ok;

            var character_id2 = dojo.felt_from_hex_be(new CString("0x01")).ok;
            var player_id2 = dojo.felt_from_hex_be(new CString("0x02")).ok;

            PlayerCharacter[] playerCharacters = new PlayerCharacter[2];
            PlayerCharacter p1 = new PlayerCharacter();
            p1.player = dojo.felt_from_hex_be(new CString("0x01")).ok;
            p1.character_id = dojo.felt_from_hex_be(new CString("0x02")).ok;

            PlayerCharacter p2 = new PlayerCharacter();
            p2.player = dojo.felt_from_hex_be(new CString("0x02")).ok;
            p2.character_id = dojo.felt_from_hex_be(new CString("0x00")).ok;

            playerCharacters[0] = p1;
            playerCharacters[1] = p2;

            // dojo.FieldElement[] calldata = new dojo.FieldElement[playerCharacters.Length * 2];

            // for (int i = 0; i < playerCharacters.Length; i++)
            // {
            //     calldata[i * 2] = playerCharacters[i].player;
            //     calldata[i * 2 + 1] = playerCharacters[i].character_id;
            // }

            List<dojo.FieldElement> calldata = new List<dojo.FieldElement>();

            for (int i = 0; i < playerCharacters.Length; i++)
            {
                calldata.Add(playerCharacters[i].player);
                calldata.Add(playerCharacters[i].character_id);
            }

            dojo.Call call = new dojo.Call()
            {
                calldata = new[]
                {
                   dojo.felt_from_hex_be(new CString("0x02")).ok, player_id, character_id, player_id2, character_id2
                },
                to = actionsAddress,
                selector = "init"
            };

            account.ExecuteRaw(new[] { call });
        }
    }


    private void move()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            string rpcUrl = "http://localhost:5050";

            var provider = new JsonRpcClient(rpcUrl);
            var signer = new SigningKey("0x1800000000300000180000000000030000000000003006001800006600");
            string playerAddress = "0x517ececd29116499f4a1b64b094da79ba08dfd54a3edaa316134c41f8160973";

            var account = new Account(provider, signer, playerAddress);
            string actionsAddress = "0xf95f269a39505092b2d4eea3268e2e8da83cfd12a20b0eceb505044ecaabf2";

            var match_id = dojo.felt_from_hex_be(new CString("0x0")).ok;
            var player_id = dojo.felt_from_hex_be(new CString("0x2")).ok;
            var character_id = dojo.felt_from_hex_be(new CString("0x4")).ok;
            var x = dojo.felt_from_hex_be(new CString("0x6")).ok;
            var y = dojo.felt_from_hex_be(new CString("0x18")).ok;

            dojo.Call call = new dojo.Call()
            {
                calldata = new[]
                {
                   match_id, player_id, character_id, x, y
                },
                to = actionsAddress,
                selector = "move"
            };

            account.ExecuteRaw(new[] { call });
        }
    }

    private void end_turn()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            string rpcUrl = "http://localhost:5050";

            var provider = new JsonRpcClient(rpcUrl);
            var signer = new SigningKey("0x1800000000300000180000000000030000000000003006001800006600");
            string playerAddress = "0x517ececd29116499f4a1b64b094da79ba08dfd54a3edaa316134c41f8160973";

            var account = new Account(provider, signer, playerAddress);
            string actionsAddress = "0x61231db30a04f42b3c3e57cd13b0dee6053f8ed7c350135735e67c254b60454";


            List<dojo.FieldElement> calldata = new List<dojo.FieldElement>();

            var match_id = dojo.felt_from_hex_be(new CString("0x0")).ok;
            var player_id = dojo.felt_from_hex_be(new CString("0x01")).ok;

            dojo.Call call = new dojo.Call()
            {
                calldata = new[]
                {
                   match_id, player_id
                },
                to = actionsAddress,
                selector = "end_turn"
            };

            account.ExecuteRaw(new[] { call });
        }
    }

    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            string rpcUrl = "http://localhost:5050";

            var provider = new JsonRpcClient(rpcUrl);
            var signer = new SigningKey("0x1800000000300000180000000000030000000000003006001800006600");
            string playerAddress = "0x517ececd29116499f4a1b64b094da79ba08dfd54a3edaa316134c41f8160973";

            var account = new Account(provider, signer, playerAddress);
            string actionsAddress = "0x68705e426f391541eb50797796e5e71ee3033789d82a8c801830bb191aa3bf1";


            List<dojo.FieldElement> calldata = new List<dojo.FieldElement>();

            var match_id = dojo.felt_from_hex_be(new CString("0x0")).ok;

            var player_id_from = dojo.felt_from_hex_be(new CString("0x01")).ok;
            var character_id_from = dojo.felt_from_hex_be(new CString("0x03")).ok;
            var skill_id = dojo.felt_from_hex_be(new CString("0x01")).ok;
            var level = dojo.felt_from_hex_be(new CString("0x01")).ok;

            var player_id_receiver = dojo.felt_from_hex_be(new CString("0x02")).ok;
            var character_id_receiver = dojo.felt_from_hex_be(new CString("0x04")).ok;

            dojo.Call call = new dojo.Call()
            {
                calldata = new[]
                {
                   match_id, player_id_from, character_id_from, skill_id, level,
                   player_id_receiver, character_id_receiver
                },
                to = actionsAddress,
                selector = "action"
            };

            account.ExecuteRaw(new[] { call });
        }
    }

    void Update()
    {
        mint();
    }
}
