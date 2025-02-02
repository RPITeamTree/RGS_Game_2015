﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MatchState { PreMatch, InMatch, PostMatch }

public class MatchManager : MonoBehaviour
{
    // General
    public Mage[] players;
    private List<Ball> balls = new List<Ball>();
    public Ball ball_prefab;
    public GGPage gg_page;

    // State and score
    MatchState state = MatchState.PreMatch;
    private int winner_player_num = -1;

    // Timers
    private float time_newball = 15f; // seconds
    private float time_delay_ggpage = 2.5f;



    // PUBLIC MODIFIERS

    public void RegisterBall(Ball ball)
    {
        balls.Add(ball);
        ball.event_ball_hit_mage += OnBallHitMage;
    }


    // PUBLIC ACCESSORS AND HELPERS

    public int GetWinnerPlayerNum()
    {
        return players[0].GetHearts() > players[1].GetHearts() ? 1 :
               players[1].GetHearts() > players[0].GetHearts() ? 2 : 0;
    }
    public Mage GetOpponent(Mage mage)
    {
        return players[GetOpponentNumber(mage.GetPlayerNumber()) - 1];
    }
    public static int GetOpponentNumber(int player_num)
    {
        return player_num == 1 ? 2 : 1; 
    }
    public MatchState GetMatchState()
    {
        return state;
    }


    // PRIVATE MODIFIERS

    private void Awake()
    {
        // players
        players[0].Inititalize(1, this);
        players[1].Inititalize(2, this);

    }
    private void Start()
    {   
        // events
        players[0].event_hearts_change += OnMageHeartsChange;
        players[1].event_hearts_change += OnMageHeartsChange;

        // begin
        StartMatch();
    }
    private void StartMatch()
    {
        state = MatchState.InMatch;
    }
    private void OnBallHitMage(Ball ball)
    {
        balls.Remove(ball);
        if (balls.Count == 0) StartCoroutine(CreateNewBall());
    }
    private void OnMageHeartsChange(Mage mage)
    {
        if (mage.GetHearts() == 0)
        {
            GG(GetOpponentNumber(mage.GetPlayerNumber()));
        }
    }

    private void GG(int winning_player_num)
    {
        state = MatchState.PostMatch;
        //match_audio.PlayGameOver();
        TimeScaleManager.Instance.AddMultiplier("GG_slow", 0.3f);
        this.winner_player_num = winning_player_num;

        StartCoroutine(TranInGGPage());
    }

    private IEnumerator CreateNewBall()
    {
        yield return new WaitForSeconds(time_newball);
        Instantiate(ball_prefab);
    }
    private IEnumerator TranInGGPage()
    {
        yield return new WaitForSeconds(time_delay_ggpage);
        gg_page.TransitionIn();
    }

}
