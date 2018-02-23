/*  Measure strength magnitude by conducting just-noticeable difference study
 *  - 3 baseline angle: 20 70 120
 *  - 3 offset angle: 0 20 45 
 *  - 2 or 3 reversal?
 *  - 9 trials for one participant, stimuli are randomized
 *  - the interval of consecutive stimulus  pair is 15 sec
 *  - the interval in a stimulus pair is 2 sec
 *  - consider about speed? 128 255
 *  - seperate into 3 region? left, right, both
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

	// user record
	public string userName = "user_no";
	private StreamWriter sw;

	// motor parameter
	private int motorSpeed = 200;
	private int[] baselineAngle = { 20, 70, 120 };
	private int[] offsetAngle = { 0, 20, 45 };

	public struct TrialPair
	{
	    public string baselineTrial;
		public string offsetTrial;
	};
	List<TrialPair> allTrials = new List<TrialPair>();
	static int[] randomizedTrialNo = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

	// Arduino connection
	private CommunicateWithArduino Uno = new CommunicateWithArduino();

	//timing
	TimeSpan prevTaskTime = DateTime.Now.TimeOfDay;
	bool timerStart = false;
	bool stimTimerStart = false;
	Text timerTextDisplay;
	Text taskTextDisplay;
	float remainingTime = 0;
	static int interval = 15;
	bool stimTimer = false;
	int stimTime = 4;
	int stimDelay = 2;
	bool stimSent = true;
	int taskNum = 1;
	int tasks = 9;
	int response;
	float responseTime;

	// Use this for initialization
	void Start()
	{

		new Thread(Uno.connectToArdunio).Start();

		//sw = new StreamWriter("JND_results\\" + userName + ".csv", true);

		coverImage.enabled = false;
		finishedText.enabled = false;
		finishedImage.enabled = false;
		sw.WriteLine("trial_no", "baseline_angle", "offset_angle", "response", "response_time");
		Debug.Log("Start");

		initializeTrials();
		generateTrialNo();

	}

	// Update is called once per frame
	void Update()
	{
		Debug.Log("start print");
		foreach (TrialPair p in allTrials)
		{
			Debug.Log(p.baselineTrial);
			Debug.Log(p.offsetTrial);
		}
		Debug.Log("stop print");
		for (int i = 0; i < randomizedTrialNo.Length; i++)
		{
			Debug.Log(randomizedTrialNo[i]);
		}

		if (timerStart)
		{
			remainingTime -= (Time.fixedDeltaTime * (float)0.6);
			Debug.Log(Mathf.Abs((DateTime.Now.TimeOfDay.Seconds) - prevTaskTime.Seconds));
			Debug.Log(remainingTime);
			updateTimerText((int)remainingTime);
			if (remainingTime < 1)
			{
				remainingTime = 0;
				timerStart = false;
				startButton.interactable = true;
			}
		}
		//Debug.Log("---------Start UPDATE---------");
		if (stimTimer)
		{
			int stimremainingTime = stimTime - Mathf.Abs((DateTime.Now.TimeOfDay.Seconds) - prevTaskTime.Seconds);
			//Debug.Log ("stimremainingTime : " + stimremainingTime + " timeSent : " + stimSent);
			if (!stimSent)
			{
				if (Mathf.Abs(stimremainingTime - stimDelay) < 1)
				{
					//sendData(task[randomizedOrder[taskNum]]);
					stimSent = true;
				}
			}
			//Debug.Log (stimremainingTime + ": " + debugCount++);
			if (stimremainingTime < 1)
			{
				stimTimer = false;
				coverImage.enabled = false;
			}
		}

	}

	public void startButtonClick()
	{
		// When start button is clicked, send the first stimulus to UNO, after a 2 sec interval, send the second stimulus to UNO. 
		// if user wants to repeat, she/he may allow to click repeat button once.
		Debug.Log("startButton");
		coverImage.enabled = true;
		// stim timer may be different
		stimTimer = true;
		prevTaskTime = DateTime.Now.TimeOfDay;
		stimSent = false;

		confirmButton.interactable = true;
		repeatButton.interactable = true;
		startButton.interactable = true;
	}

	public void confirmButtonClick()
	{
		// When confirm button is clicked, record response, wait for a 15 sec interval.
		// After the interval, continue next trial.
		Debug.Log("confirmButton");
		writeData();
		timerStart = true;
		prevTaskTime = DateTime.Now.TimeOfDay;
		confirmButton.interactable = false;
		startButton.interactable = false;
		if (taskNum == 9)
		{
			finishedText.enabled = true;
			finishedImage.enabled = true;
			Debug.Log("Finished");
		}
		taskNum++;
		updateTaskText(taskNum);
		remainingTime = 15;
	}

	public void repeatButtonClick()
	{
		Debug.Log("repeat button");
		repeatButton.interactable = false;
		coverImage.enabled = true;
		// stim timer may be different
		stimTimer = true;
		prevTaskTime = DateTime.Now.TimeOfDay;
		stimSent = false;
	}

	void updateTimerText(int time)
	{
		timerTextDisplay = GameObject.Find("timeText").GetComponent<Text>();
		timerTextDisplay.text = "Time: " + time.ToString() + "s";
	}

	void updateTaskText(int task)
	{
		taskTextDisplay = GameObject.Find("trialText").GetComponent<Text>();
		taskTextDisplay.text = "Task No: " + (taskNum).ToString() + "/" + tasks;
	}

	public void writeData()
	{
		Debug.Log("writeData");
		writeFile(taskNum.ToString() + "," + allTrials[randomizedTrialNo[taskNum]].baselineTrial + "," +
			allTrials[randomizedTrialNo[taskNum]].offsetTrial + "," +
			response.ToString() + "," + responseTime.ToString() + "\n");
	}

	IEnumerator sendFirstData(int trialNum)
	{
		float time = 1.0f;
		Debug.Log ("sendFirstData");
		string data = allTrials[randomizedTrialNo[trialNum]].baselineTrial;
		Debug.Log(data);
		if (Uno != null)
		{
			new Thread(Uno.SendData).Start(data);
			yield return new WaitForSeconds(time);
			Debug.Log("Time's up!");
		}
		else
		{
			Debug.Log("nullport");
		}
	}

	IEnumerator sendSecondData(int trialNum)
	{
		float time = 1.0f;
		Debug.Log("sendSecondData");
		string data = allTrials[randomizedTrialNo[trialNum]].offsetTrial;
		Debug.Log(data);
		if (Uno != null)
		{
			new Thread(Uno.SendData).Start(data);
			yield return new WaitForSeconds(time);
			Debug.Log("Time's up!");
		}
		else
		{
			Debug.Log("nullport");
		}
	}

	void writeFile(string data)
	{
		string fileName = "JND_results\\" + userName + ".csv";
		sw = new StreamWriter(fileName, true);
		sw.Write(data);
		sw.Flush();
		sw.Close();
		sw.Dispose();
	}

	private int degreeConvertToRotaryCoder(int degree)
	{
		// alternation
		// increase another converter for right motor
		return (degree * 1024 / 360);
	}

	private void initializeTrials()
	{
		// generate all combinations of basline angle and offset angle, combine them into pairs and store in allTrials
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				TrialPair temp;
				int convertBaseline = degreeConvertToRotaryCoder(baselineAngle[i]);
				int convertOffset = degreeConvertToRotaryCoder(offsetAngle[j]);

				temp.baselineTrial = convertBaseline + " " + motorSpeed + " " + convertBaseline + " " + motorSpeed;
				temp.offsetTrial = convertOffset + " " + motorSpeed + " " + convertOffset + " " + motorSpeed;
				allTrials.Add(temp);
			}
		}

	}

	private void generateTrialNo()
	{
		// generate randomized trial sequence
		for (int i = 0; i < randomizedTrialNo.Length; i++)
		{
			int tmp = randomizedTrialNo[i];
			int randNum = UnityEngine.Random.Range(i, randomizedTrialNo.Length);
			randomizedTrialNo[i] = randomizedTrialNo[randNum];
			randomizedTrialNo[randNum] = tmp;
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
				string portChoice = "COM2";
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
