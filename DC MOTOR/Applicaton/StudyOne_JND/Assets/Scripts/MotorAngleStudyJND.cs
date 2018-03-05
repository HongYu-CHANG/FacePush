/*  Measure strength magnitude by conducting just-noticeable difference study
 *  Issues to go:
 *  - 3 baseline angle? 20 70 120
 *  - 3 offset angle? 0 20 45 
 *  - seperate into 3 region? left, right, both
 *  - 2 or 3 reversal? 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class MotorAngleStudyJND : MonoBehaviour
{
	// canvas components
	public Button startButton;
	public Button confirmButton;
	public Button repeatButton;
	public Image finishedImage;
	public Image coverImage;
	public Text finishedText;
	public Toggle yesToggle;
	public Toggle noToggle;

	// user record
	public string userName = "user_no";
	private StreamWriter fileWriter;

	// motor angle parameter
	private int motorSpeed = 200;
	private int[] baselineAngle = { 50, 75, 100, 150 };
	private int[] offsetAngle = { 20, 50, 110, 170 };

	// Arduino connection
	private CommunicateWithArduino Uno = new CommunicateWithArduino();

	// timing
	TimeSpan prevTaskTime = DateTime.Now.TimeOfDay;
	TimeSpan endStimTime;
	Text timerTextDisplay;
	Text taskTextDisplay;
	bool timerStart = false;
	bool stimTimer = false;
	bool stimFirstSent = true;
	bool stimSecondSent = true;
	bool backToOrigin = false;
	float remainingTime = 0;
	float responseTime;
	int stimTime = 10;
	int stimDelayFirst = 9;
	int backToOriginDelay = 7;
	int stimDelaySecond = 5;
	public int interval = 15;

	// trial setting
	public struct TrialPair
	{
		public string baselineTrial;
		public string offsetTrial;
	};
	List<TrialPair> allTrials = new List<TrialPair>();
	static int[] allRandomizedTrialNo;
    int blocks = 1;
	int taskNum = 0;
	int totalTasks;
	string response;

	// Use this for initialization
	void Start()
	{
		new Thread(Uno.connectToArdunio).Start();
		writeFile("trial_no,baseline_angle,offset_angle,same,response_time\n");
		coverImage.enabled = false;
		finishedText.enabled = false;
		finishedImage.enabled = false;
		repeatButton.interactable = false;
		confirmButton.interactable = false;
		initializeTrials();
		totalTasks = blocks * allTrials.Count;
		generateTrialNo();
		Debug.Log("Start");
	}

	// Update is called once per frame
	void Update()
	{
		if (timerStart)
		{
			remainingTime -= (Time.fixedDeltaTime * (float) 0.6);
			//Debug.Log(Mathf.Abs((DateTime.Now.TimeOfDay.Seconds) - prevTaskTime.Seconds));
			//Debug.Log(remainingTime);
			updateTimerText((int)remainingTime);
			if (remainingTime < 1)
			{
				remainingTime = 0;
				timerStart = false;
				startButton.interactable = true;
			}
		}

		if (stimTimer)
		{
			int stimRemainingTime = stimTime - Mathf.Abs((DateTime.Now.TimeOfDay.Seconds) - prevTaskTime.Seconds);
			//Debug.Log("stimRemainingTime : " + stimRemainingTime);

			if (backToOrigin)
			{
				if (Mathf.Abs(stimRemainingTime - backToOriginDelay) < 1)
				{
					new Thread(Uno.SendData).Start("0 200 0 200");
					backToOrigin = false;
				}
			}

			if (!stimFirstSent)
			{
				if (Mathf.Abs(stimRemainingTime - stimDelayFirst) < 1) // 1
				{
					Debug.Log("in sending first data");
					new Thread(Uno.SendData).Start(allTrials[allRandomizedTrialNo[taskNum]].baselineTrial.ToString());
					stimFirstSent = true;
					backToOrigin = true;
					backToOriginDelay = 7;
				}
			}

			if (!stimSecondSent)
			{
				if (Mathf.Abs(stimRemainingTime - stimDelaySecond) < 1)
				{
					Debug.Log("in sending second data");
					new Thread(Uno.SendData).Start(allTrials[allRandomizedTrialNo[taskNum]].offsetTrial.ToString());
					stimSecondSent = true;
					backToOrigin = true;
					backToOriginDelay = 3;
				}
			}
			
			if (stimRemainingTime < 1)
			{
				stimTimer = false;
				coverImage.enabled = false;
				endStimTime = DateTime.Now.TimeOfDay;
			}
		}
	}

	public void startButtonClick()
	{
		// When start button is clicked, send the first stimulus to UNO, after a 2 sec interval, send the second stimulus to UNO. 
		// if user wants to repeat, she/he may allow to click repeat button once.
		Debug.Log("startButton");
		coverImage.enabled = true;
		stimTimer = true;
		prevTaskTime = DateTime.Now.TimeOfDay;
		stimFirstSent = false;
		stimSecondSent = false;
		confirmButton.interactable = true;
		repeatButton.interactable = true;
		startButton.interactable = false;
	}

	public void confirmButtonClick()
	{
		// write data
		Debug.Log("confirmButton");
		response = checkResponse();
		Debug.Log(response);
		responseTime = checkResponseTime();
		Debug.Log(responseTime);
		writeData();

		// update timer and UI
		timerStart = true;
		prevTaskTime = DateTime.Now.TimeOfDay;
		startButton.interactable = false;
		confirmButton.interactable = false;
		repeatButton.interactable = false;
		yesToggle.isOn = false;
		noToggle.isOn = false;
		if (taskNum + 1 == totalTasks)
		{
			finishedText.enabled = true;
			finishedImage.enabled = true;
			Debug.Log("Finished");
		}
		else
		{
			taskNum++;
		}
		updateTaskText(taskNum + 1);
		remainingTime = interval;
	}

	public void repeatButtonClick()
	{
		Debug.Log("repeat button");
		repeatButton.interactable = false;
		coverImage.enabled = true;
		stimTimer = true;
		prevTaskTime = DateTime.Now.TimeOfDay;
		stimFirstSent = false;
		stimSecondSent = false;
	}

	void updateTimerText(int time)
	{
		timerTextDisplay = GameObject.Find("timeText").GetComponent<Text>();
		timerTextDisplay.text = "Time: " + time.ToString() + "s";
	}

	void updateTaskText(int task)
	{
		taskTextDisplay = GameObject.Find("trialText").GetComponent<Text>();
		taskTextDisplay.text = "Task No: " + (taskNum + 1).ToString() + "/" + totalTasks;
	}

	public void writeData()
	{
		Debug.Log("writeData");
		writeFile(taskNum.ToString() + "," + allTrials[allRandomizedTrialNo[taskNum]].baselineTrial + "," +
			allTrials[allRandomizedTrialNo[taskNum]].offsetTrial + "," +
			response + "," + responseTime.ToString() + "\n");
	}

	void writeFile(string data)
	{
		string fileName = "JND_results\\" + userName + ".csv";
		fileWriter = new StreamWriter(fileName, true);
		fileWriter.Write(data);
		fileWriter.Flush();
		fileWriter.Close();
		fileWriter.Dispose();
	}

	private int degreeConvertToLeftRotaryCoder(int degree)
	{
		// alternation
		// increase another converter for right motor
		return (degree * 1024 / 360);
	}

	private int degreeConvertToRightRotaryCoder(int degree)
	{
		// alternation
		// increase another converter for right motor
		return (degree *  682/ 360);
	}

	private string checkResponse()
	{
		if (yesToggle.isOn && !noToggle.isOn)
		{
			return "1";
		}
		else if (!yesToggle.isOn && noToggle.isOn)
		{
			return "0";
		}
		else
		{
			return "false_response";
		}
	}

	private int checkResponseTime()
	{
		int RTInSecond = Mathf.Abs((DateTime.Now.TimeOfDay.Seconds) - endStimTime.Seconds);
		int RTInMillisecond = Mathf.Abs((DateTime.Now.TimeOfDay.Milliseconds) - endStimTime.Milliseconds);
		return RTInSecond * 1000 + RTInMillisecond;
	}

	private void initializeTrials()
	{
		// generate all combinations of basline angle and offset angle, combine them into pairs and store in allTrials
		// if we want to contain location, add them here
		for (int i = 0; i < baselineAngle.Length; i++)
		{
			for (int j = 0; j < offsetAngle.Length; j++)
			{
				TrialPair temp;
				int convertLeftBaseline = degreeConvertToLeftRotaryCoder(baselineAngle[i]);
				int convertLeftOffset = degreeConvertToLeftRotaryCoder(offsetAngle[j]);
				int convertRightBaseline = degreeConvertToRightRotaryCoder(baselineAngle[i]);
				int convertRightOffset = degreeConvertToRightRotaryCoder(offsetAngle[j]);
				temp.baselineTrial = convertLeftBaseline + " " + motorSpeed + " " + convertRightBaseline + " " + motorSpeed;
				temp.offsetTrial = convertLeftOffset + " " + motorSpeed + " " + convertRightOffset + " " + motorSpeed;
				allTrials.Add(temp);
			}
		}
	}

	private void generateTrialNo()
	{
		// generate randomized trial sequence
		allRandomizedTrialNo = new int[totalTasks];
		int[] trialNo = new int[allTrials.Count];
		int num = 0;
		for (int i = 0; i < allTrials.Count; i++) trialNo[i] = i;

		for (int k = 0; k < blocks; k++)
		{
			for (int i = 0; i < trialNo.Length; i++)
			{
				int tmp = trialNo[i];
				int randNum = UnityEngine.Random.Range(i, trialNo.Length);
				trialNo[i] = trialNo[randNum];
				trialNo[randNum] = tmp;
				allRandomizedTrialNo[num] = trialNo[i];
				num++;
			}
		}
	}

	class CommunicateWithArduino
	{
		public bool connected = true;
		public bool mac = false;
		public string choice = "cu.usbmodem1421";
		private SerialPort arduinoController;

		public void connectToArdunio()
		{

			if (connected)
			{
				string portChoice = "COM8";
				if (mac)
				{
					int p = (int)Environment.OSVersion.Platform;
					// Are we on Unix?
					if (p == 4 || p == 128 || p == 6)
					{
						List<string> serial_ports = new List<string>();
						string[] ttys = Directory.GetFiles("/dev/", "cu.*");
						foreach (string dev in ttys)
						{
							if (dev.StartsWith("/dev/tty."))
							{
								serial_ports.Add(dev);
								Debug.Log(String.Format(dev));
							}
						}
					}
					portChoice = "/dev/" + choice;
				}
				arduinoController = new SerialPort(portChoice, 57600, Parity.None, 8, StopBits.One);
				arduinoController.Handshake = Handshake.None;
				arduinoController.RtsEnable = true;
				arduinoController.Open();
				Debug.LogWarning(arduinoController);
			}

		}
		public void SendData(object obj)
		{
			string data = obj as string;
			Debug.Log(data);
			if (connected)
			{
				if (arduinoController != null)
				{
					arduinoController.Write(data);
					arduinoController.Write("\n");
				}
				else
				{
					Debug.Log(arduinoController);
					Debug.Log("nullport");
				}
			}
			else
			{
				Debug.Log("not connected");
			}
			Thread.Sleep(500);
		}
	}

}
