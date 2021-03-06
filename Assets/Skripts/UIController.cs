﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour {

    public GameObject nogata;
    private int currentLevel;
    private int levelCount;

	// Use this for initialization
	void Start () {
        currentLevel = 1;
        levelCount = SceneManager.sceneCountInBuildSettings - 1;

        Button shotButton = transform.FindChild("IngamePanel").FindChild("ShotButton").GetComponent<Button>();
        shotButton.onClick.AddListener(Shot);

        Button stopButton = transform.FindChild("IngamePanel").FindChild("StopButton").GetComponent<Button>();
        stopButton.onClick.AddListener(Stop);

        Button nextShotButton = transform.FindChild("ShotFinishedPanel").FindChild("NextShotButton").GetComponent<Button>();
        nextShotButton.onClick.AddListener(NextShot);

        Button finishButton = transform.FindChild("ShotFinishedPanel").FindChild("FinishButton").GetComponent<Button>();
        finishButton.onClick.AddListener(FinishLevel);

        Button retryButton = transform.FindChild("LevelFinishedPanel").FindChild("RetryButton").GetComponent<Button>();
        retryButton.onClick.AddListener(Retry);

        Button levelMenuButton = transform.FindChild("LevelFinishedPanel").FindChild("LevelMenuButton").GetComponent<Button>();
        levelMenuButton.onClick.AddListener(OpenLevelMenu);

        Button startButton = transform.FindChild("MainMenuPanel").FindChild("StartButton").GetComponent<Button>();
        startButton.onClick.AddListener(OpenLevelMenu);

        Button exitButton = transform.FindChild("MainMenuPanel").FindChild("ExitButton").GetComponent<Button>();
        exitButton.onClick.AddListener(Exit);

        Button startLevelButton = transform.FindChild("LevelMenuPanel").FindChild("StartButton").GetComponent<Button>();
        startLevelButton.onClick.AddListener(StartLevel);

        Button nextLevelButton = transform.FindChild("LevelMenuPanel").FindChild("NextButton").GetComponent<Button>();
        nextLevelButton.onClick.AddListener(NextLevel);

        Button prevLevelButton = transform.FindChild("LevelMenuPanel").FindChild("BackButton").GetComponent<Button>();
        prevLevelButton.onClick.AddListener(PrevLevel);

        Button mainMenuButton = transform.FindChild("LevelMenuPanel").FindChild("MainMenuButton").GetComponent<Button>();
        mainMenuButton.onClick.AddListener(OpenMainMenu);

        RefreshScore();

        if (SceneManager.GetActiveScene().name.StartsWith("Level"))
        {
            ShowIngameUI();
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void Shot()
    {
        nogata.GetComponent<NogataController>().Shot();
        GameObject.Find("GameController").GetComponent<GameController>().ShotStarted();
        transform.FindChild("IngamePanel").FindChild("ShotButton").gameObject.SetActive(false);
        transform.FindChild("IngamePanel").FindChild("StopButton").gameObject.SetActive(true);
    }

    void Stop()
    {
        Destroy(GameObject.Find("Nogata"));
        GameObject.Find("GameController").GetComponent<GameController>().ShotFinished(false);
    }

    void NextShot()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RefreshScore()
    {
        transform.FindChild("IngamePanel").FindChild("ScoreUI").GetComponent<Text>().text = "Stars: " + GameObject.Find("GameController").GetComponent<GameController>().GetData().getStarCount();
        transform.FindChild("IngamePanel").FindChild("MeteoritesUI").GetComponent<Text>().text = "Meteorites: " + GameObject.Find("GameController").GetComponent<GameController>().GetData().meteoritesLeft;
    }

    public void ShowShotFinishedPanel()
    {
        transform.FindChild("IngamePanel").gameObject.SetActive(false);

        Data data = GameObject.Find("GameController").GetComponent<GameController>().GetData();
        transform.FindChild("ShotFinishedPanel").FindChild("ScoreSummary").GetComponent<Text>().text = "Stars: " + data.getStarCount() + " (x2)\n";
        transform.FindChild("ShotFinishedPanel").FindChild("ScoreSummary").GetComponent<Text>().text += "Meteorites left: " + data.meteoritesLeft;
        transform.FindChild("ShotFinishedPanel").FindChild("NoScoreWarning").gameObject.SetActive(!data.earthHit);
        transform.FindChild("ShotFinishedPanel").FindChild("WholeScore").gameObject.SetActive(data.earthHit);
        transform.FindChild("ShotFinishedPanel").FindChild("WholeScore").GetComponent<Text>().text = "Score: " + (data.getStarCount() * 2 + data.meteoritesLeft);
        transform.FindChild("ShotFinishedPanel").FindChild("NextShotButton").gameObject.SetActive(data.meteoritesLeft > 0 && data.getStarCount() < 3);

        transform.FindChild("ShotFinishedPanel").gameObject.SetActive(true);
    }

    void Retry()
    {
        GameObject.Find("GameController").GetComponent<GameController>().Retry();
    }

    void FinishLevel()
    {
        transform.FindChild("ShotFinishedPanel").gameObject.SetActive(false);

        Data data = GameObject.Find("GameController").GetComponent<GameController>().GetData();
        int score = (data.getStarCount() * 2 + data.meteoritesLeft);
        if (!data.earthHit)
            score = 0;
        transform.FindChild("LevelFinishedPanel").FindChild("Score").GetComponent<Text>().text = "Score: " + score;
        if (score > data.getCurrentHighscore())
        {
            transform.FindChild("LevelFinishedPanel").FindChild("Highscore").GetComponent<Text>().text = "New Highscore!";
            data.setCurrentHighscore(score);
        }
        else
        {
            transform.FindChild("LevelFinishedPanel").FindChild("Highscore").GetComponent<Text>().text = "Current highscore: " + data.getCurrentHighscore();
        }

        transform.FindChild("LevelFinishedPanel").gameObject.SetActive(true);
    }

    void OpenLevelMenu()
    {
        transform.FindChild("MainMenuPanel").gameObject.SetActive(false);
        transform.FindChild("LevelFinishedPanel").gameObject.SetActive(false);
        transform.FindChild("LevelMenuPanel").gameObject.SetActive(true);
        RefreshLevelMenu();
    }

    void Exit()
    {
        Application.Quit();
    }

    void NextLevel()
    {
        currentLevel++;
        RefreshLevelMenu();
    }

    void PrevLevel()
    {
        currentLevel--;
        RefreshLevelMenu();
    }

    void RefreshLevelMenu()
    {
        transform.FindChild("LevelMenuPanel").FindChild("LevelPanel").FindChild("LevelName").GetComponent<Text>().text = "Level " + currentLevel;
        Data data = GameObject.Find("GameController").GetComponent<GameController>().GetData();
        transform.FindChild("LevelMenuPanel").FindChild("LevelPanel").FindChild("Highscore").GetComponent<Text>().text = "Highscore: " + data.highscores[currentLevel - 1];
        transform.FindChild("LevelMenuPanel").FindChild("BackButton").gameObject.SetActive(currentLevel > 1);
        transform.FindChild("LevelMenuPanel").FindChild("NextButton").gameObject.SetActive(currentLevel < levelCount);
    }

    void StartLevel()
    {
        GameObject.Find("GameController").GetComponent<GameController>().GetData().resetIngame();
        SceneManager.LoadScene("Level" + currentLevel);
        transform.FindChild("LevelMenuPanel").gameObject.SetActive(false);
        transform.FindChild("IngamePanel").gameObject.SetActive(true);
    }

    public void ShowIngameUI()
    {
        transform.FindChild("MainMenuPanel").gameObject.SetActive(false);
        transform.FindChild("IngamePanel").gameObject.SetActive(true);
    }

    void OpenMainMenu()
    {
        transform.FindChild("LevelMenuPanel").gameObject.SetActive(false);
        transform.FindChild("MainMenuPanel").gameObject.SetActive(true);
    }
}
