using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System.Linq;
using KModkit;
using System;



public class binaryGridScript : MonoBehaviour
{

	public KMAudio audio;
	public KMBombInfo bomb;

	//selectables
	public KMSelectable button11;
	public KMSelectable button12;
	public KMSelectable button13;
	public KMSelectable button14;
	public KMSelectable button15;
	public KMSelectable button21;
	public KMSelectable button22;
	public KMSelectable button23;
	public KMSelectable button24;
	public KMSelectable button25;
	public KMSelectable button31;
	public KMSelectable button32;
	public KMSelectable button33;
	public KMSelectable button34;
	public KMSelectable button35;
	public KMSelectable button41;
	public KMSelectable button42;
	public KMSelectable button43;
	public KMSelectable button44;
	public KMSelectable button45;
	public KMSelectable button51;
	public KMSelectable button52;
	public KMSelectable button53;
	public KMSelectable button54;
	public KMSelectable button55;

	public KMSelectable buttonReset;
	public KMSelectable buttonCheck;
	public KMSelectable buttonSubmit;

	public Material ledColor;
	public Material ledUnlit;
	public Renderer led0;
	public Renderer led1;
	public Renderer led2;
	public Renderer led3;
	public Renderer led4;

	public Renderer ledValid1;
	public Renderer ledValid2;
	public Renderer ledValid3;

	public int[,,] patterns;
	public int selectedPattern;

	public KMSelectable[,] matrix;
	public int[,] valOrigine;
	public String[,] stringOrigine;

	public int nb1 = 0;
	public int nb0 = 0;

	public int[] key;
	public int[] key2;
	public int[] solution;

	public int nbSolved = 0;

	public bool submitMode;
	public Material submitGreen;
	public Material submitRed;
	public Renderer submit;

	//logging
	static int moduleIdCounter = 1;
	int moduleId;
	private bool moduleSolved;

	void Awake()
	{
		moduleId = moduleIdCounter++;

		submitMode = false;

		matrix = new KMSelectable[5, 5] {
			{button11,button12,button13,button14,button15},
			{button21,button22,button23,button24,button25},
			{button31,button32,button33,button34,button35},
			{button41,button42,button43,button44,button45},
			{button51,button52,button53,button54,button55}};



		//Buttons
		button11.OnInteract += delegate () {
			PressButtonGrid(0, 0);
			return false;
		};
		button12.OnInteract += delegate () {
			PressButtonGrid(0, 1);
			return false;
		};
		button13.OnInteract += delegate () {
			PressButtonGrid(0, 2);
			return false;
		};
		button14.OnInteract += delegate () {
			PressButtonGrid(0, 3);
			return false;
		};
		button15.OnInteract += delegate () {
			PressButtonGrid(0, 4);
			return false;
		};
		button21.OnInteract += delegate () {
			PressButtonGrid(1, 0);
			return false;
		};
		button22.OnInteract += delegate () {
			PressButtonGrid(1, 1);
			return false;
		};
		button23.OnInteract += delegate () {
			PressButtonGrid(1, 2);
			return false;
		};
		button24.OnInteract += delegate () {
			PressButtonGrid(1, 3);
			return false;
		};
		button25.OnInteract += delegate () {
			PressButtonGrid(1, 4);
			return false;
		};
		button31.OnInteract += delegate () {
			PressButtonGrid(2, 0);
			return false;
		};
		button32.OnInteract += delegate () {
			PressButtonGrid(2, 1);
			return false;
		};
		button33.OnInteract += delegate () {
			PressButtonGrid(2, 2);
			return false;
		};
		button34.OnInteract += delegate () {
			PressButtonGrid(2, 3);
			return false;
		};
		button35.OnInteract += delegate () {
			PressButtonGrid(2, 4);
			return false;
		};
		button41.OnInteract += delegate () {
			PressButtonGrid(3, 0);
			return false;
		};
		button42.OnInteract += delegate () {
			PressButtonGrid(3, 1);
			return false;
		};
		button43.OnInteract += delegate () {
			PressButtonGrid(3, 2);
			return false;
		};
		button44.OnInteract += delegate () {
			PressButtonGrid(3, 3);
			return false;
		};
		button45.OnInteract += delegate () {
			PressButtonGrid(3, 4);
			return false;
		};
		button51.OnInteract += delegate () {
			PressButtonGrid(4, 0);
			return false;
		};
		button52.OnInteract += delegate () {
			PressButtonGrid(4, 1);
			return false;
		};
		button53.OnInteract += delegate () {
			PressButtonGrid(4, 2);
			return false;
		};
		button54.OnInteract += delegate () {
			PressButtonGrid(4, 3);
			return false;
		};
		button55.OnInteract += delegate () {
			PressButtonGrid(4, 4);
			return false;
		};

		InitPattern();
	}


	// Use this for initialization
	void Start()
	{

		float scalar = transform.lossyScale.x;

		submit.transform.GetChild(2).GetComponent<Light>().range *= scalar;
		led0.transform.GetChild(0).GetComponent<Light>().range *= scalar;
		led1.transform.GetChild(0).GetComponent<Light>().range *= scalar;
		led2.transform.GetChild(0).GetComponent<Light>().range *= scalar;
		led3.transform.GetChild(0).GetComponent<Light>().range *= scalar;
		led4.transform.GetChild(0).GetComponent<Light>().range *= scalar;

		ledValid1.transform.GetChild(0).GetComponent<Light>().range *= scalar;
		ledValid2.transform.GetChild(0).GetComponent<Light>().range *= scalar;
		ledValid3.transform.GetChild(0).GetComponent<Light>().range *= scalar;

		CreatePuzzle();

		buttonReset.OnInteract += delegate () {
			ResetGrid();
			return false;
		};

		buttonCheck.OnInteract += delegate () {
			Check();
			return false;
		};

		buttonSubmit.OnInteract += delegate () {
			Submit();
			return false;
		};
	}

	void PressButtonGrid(int row, int col)
	{

		if (moduleSolved)
			return;

		matrix[row, col].AddInteractionPunch(.5f);
		GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);

		if (submitMode == true)
		{
			Debug.LogFormat("[Binary Grid #{0}] Stage {1} : You submitted ({2},{3}).", moduleId, nbSolved + 1, row + 1, col + 1);
			if (row == solution[0] && col == solution[1])
			{
				//solved
				nbSolved++;
				LightLedValid(nbSolved);
				if (nbSolved == 3)
				{
					moduleSolved = true;
					ResetModule();
					GetComponent<KMBombModule>().HandlePass();
				}
				else
				{

					StartCoroutine(NewPuzzle());

				}
			}
			else
			{
				//strike
				GetComponent<KMBombModule>().HandleStrike();
			}
		}
		else
		{

			if (GetValue(row, col) == "0")
				SetValue(row, col, "1");
			else
				SetValue(row, col, "0");
		}
	}

	void ResetGrid()
	{

		if (moduleSolved)
			return;

		buttonReset.AddInteractionPunch();
		GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);

		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				SetValue(i, j, valOrigine[i, j].ToString());
			}
		}
	}

	void Check()
	{

		if (moduleSolved)
			return;

		buttonCheck.AddInteractionPunch();
		GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);

		int valid = 0;

		for (int i = 0; i < 6; i++)
		{
			if (int.Parse(GetValue(patterns[selectedPattern, i, 0], patterns[selectedPattern, i, 1]))
				== key[i])
			{
				valid++;
			}
		}



		if (valid == 6)
		{
			//led
			LightLed(solution[1]);
			GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.Switch, transform);

		}
		else
		{
			//strike
			//GetComponent<KMBombModule> ().HandleStrike ();
			GetComponent<KMBombModule>().HandleStrike();
		}

		valid = 0;
	}

	string GetValue(int row, int col)
	{
		return matrix[row, col].transform.GetChild(1).GetComponent<TextMesh>().text;
	}

	void SetValue(int row, int col, string value)
	{
		matrix[row, col].transform.GetChild(1).GetComponent<TextMesh>().text = value;
	}

	void KeyGenerator()
	{
		key = new int[6];

		if (nb1 % 2 == 0)
		{
			key[0] = 1;
			key[3] = 0;
		}
		else
		{
			key[0] = 0;
			key[3] = 1;
		}

		if (nbSolved == 0)
		{
			if (bomb.GetBatteryCount() % 2 == 0)
			{
				key[1] = 1;
				key[4] = 0;
			}
			else
			{
				key[1] = 0;
				key[4] = 1;
			}

			if (nb1 > nb0)
			{
				key[2] = 1;
				key[5] = 0;
			}
			else
			{
				key[2] = 0;
				key[5] = 1;
			}
		}

		if (nbSolved == 1)
		{
			if (bomb.GetPortPlateCount() > 1)
			{
				key[1] = 1;
				key[4] = 0;
			}
			else
			{
				key[1] = 0;
				key[4] = 1;
			}

			if (nb1 < nb0)
			{
				key[2] = 1;
				key[5] = 0;
			}
			else
			{
				key[2] = 0;
				key[5] = 1;
			}
		}

		if (nbSolved == 2)
		{
			if (bomb.GetOnIndicators().Count() < 2)
			{
				key[1] = 1;
				key[4] = 0;
			}
			else
			{
				key[1] = 0;
				key[4] = 1;
			}

			if (valOrigine[2, 2] == 1)
			{
				key[2] = 1;
				key[5] = 0;
			}
			else
			{
				key[2] = 0;
				key[5] = 1;
			}
		}

	}

	void SolutionGenerator()
	{
		solution = new int[2];

		int nb1keys = (count1(key) + count1(key2));
		int nb0keys = (count0(key) + count0(key2));

		int mini = Math.Min(nb0keys, nb1keys);

		if (bomb.GetStrikes() > 0)
		{
			solution[0] = mini - 2;
		}
		else { solution[0] = mini - 3; }

		System.Random rand = new System.Random();

		solution[1] = rand.Next(5);

	}

	void InitPattern()
	{
		patterns = new int[8, 6, 2];
		patterns[0, 0, 0] = 0;
		patterns[0, 0, 1] = 1;
		patterns[0, 1, 0] = 1;
		patterns[0, 1, 1] = 3;
		patterns[0, 2, 0] = 2;
		patterns[0, 2, 1] = 0;
		patterns[0, 3, 0] = 2;
		patterns[0, 3, 1] = 4;
		patterns[0, 4, 0] = 3;
		patterns[0, 4, 1] = 2;
		patterns[0, 5, 0] = 4;
		patterns[0, 5, 1] = 4;

		patterns[1, 0, 0] = 0;
		patterns[1, 0, 1] = 2;
		patterns[1, 1, 0] = 1;
		patterns[1, 1, 1] = 4;
		patterns[1, 2, 0] = 2;
		patterns[1, 2, 1] = 3;
		patterns[1, 3, 0] = 3;
		patterns[1, 3, 1] = 1;
		patterns[1, 4, 0] = 4;
		patterns[1, 4, 1] = 2;
		patterns[1, 5, 0] = 4;
		patterns[1, 5, 1] = 4;

		patterns[2, 0, 0] = 0;
		patterns[2, 0, 1] = 0;
		patterns[2, 1, 0] = 0;
		patterns[2, 1, 1] = 2;
		patterns[2, 2, 0] = 2;
		patterns[2, 2, 1] = 2;
		patterns[2, 3, 0] = 2;
		patterns[2, 3, 1] = 4;
		patterns[2, 4, 0] = 3;
		patterns[2, 4, 1] = 1;
		patterns[2, 5, 0] = 4;
		patterns[2, 5, 1] = 3;

		patterns[3, 0, 0] = 0;
		patterns[3, 0, 1] = 3;
		patterns[3, 1, 0] = 1;
		patterns[3, 1, 1] = 0;
		patterns[3, 2, 0] = 1;
		patterns[3, 2, 1] = 2;
		patterns[3, 3, 0] = 2;
		patterns[3, 3, 1] = 3;
		patterns[3, 4, 0] = 3;
		patterns[3, 4, 1] = 1;
		patterns[3, 5, 0] = 4;
		patterns[3, 5, 1] = 2;

		patterns[4, 0, 0] = 0;
		patterns[4, 0, 1] = 4;
		patterns[4, 1, 0] = 1;
		patterns[4, 1, 1] = 1;
		patterns[4, 2, 0] = 2;
		patterns[4, 2, 1] = 0;
		patterns[4, 3, 0] = 3;
		patterns[4, 3, 1] = 1;
		patterns[4, 4, 0] = 3;
		patterns[4, 4, 1] = 3;
		patterns[4, 5, 0] = 4;
		patterns[4, 5, 1] = 3;

		patterns[5, 0, 0] = 1;
		patterns[5, 0, 1] = 1;
		patterns[5, 1, 0] = 1;
		patterns[5, 1, 1] = 4;
		patterns[5, 2, 0] = 2;
		patterns[5, 2, 1] = 0;
		patterns[5, 3, 0] = 3;
		patterns[5, 3, 1] = 1;
		patterns[5, 4, 0] = 3;
		patterns[5, 4, 1] = 3;
		patterns[5, 5, 0] = 4;
		patterns[5, 5, 1] = 0;

		patterns[6, 0, 0] = 0;
		patterns[6, 0, 1] = 3;
		patterns[6, 1, 0] = 1;
		patterns[6, 1, 1] = 1;
		patterns[6, 2, 0] = 2;
		patterns[6, 2, 1] = 4;
		patterns[6, 3, 0] = 3;
		patterns[6, 3, 1] = 0;
		patterns[6, 4, 0] = 3;
		patterns[6, 4, 1] = 2;
		patterns[6, 5, 0] = 4;
		patterns[6, 5, 1] = 4;

		patterns[7, 0, 0] = 0;
		patterns[7, 0, 1] = 0;
		patterns[7, 1, 0] = 1;
		patterns[7, 1, 1] = 2;
		patterns[7, 2, 0] = 2;
		patterns[7, 2, 1] = 0;
		patterns[7, 3, 0] = 2;
		patterns[7, 3, 1] = 4;
		patterns[7, 4, 0] = 3;
		patterns[7, 4, 1] = 1;
		patterns[7, 5, 0] = 4;
		patterns[7, 5, 1] = 3;

	}

	void GetPattern()
	{
		if (key[0] == 1 && key[1] == 1 && key[2] == 1)
			selectedPattern = 0;
		if (key[0] == 1 && key[1] == 1 && key[2] == 0)
			selectedPattern = 1;
		if (key[0] == 1 && key[1] == 0 && key[2] == 0)
			selectedPattern = 2;
		if (key[0] == 0 && key[1] == 0 && key[2] == 0)
			selectedPattern = 3;
		if (key[0] == 0 && key[1] == 0 && key[2] == 1)
			selectedPattern = 4;
		if (key[0] == 0 && key[1] == 1 && key[2] == 1)
			selectedPattern = 5;
		if (key[0] == 1 && key[1] == 0 && key[2] == 1)
			selectedPattern = 6;
		if (key[0] == 0 && key[1] == 1 && key[2] == 0)
			selectedPattern = 7;
	}

	void GetKey2()
	{
		key2 = new int[6];
		for (int i = 0; i < 6; i++)
		{
			key2[i] = valOrigine[patterns[selectedPattern, i, 0], patterns[selectedPattern, i, 1]];
		}
	}

	int count1(int[] array)
	{
		int count = 0;
		foreach (int i in array)
		{
			if (i == 1) count++;
		}
		return count;
	}

	int count0(int[] array)
	{
		int count = 0;
		foreach (int i in array)
		{
			if (i == 0) count++;
		}
		return count;
	}

	void Submit()
	{

		buttonSubmit.AddInteractionPunch();
		GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);

		if (submitMode == true)
		{
			submitMode = false;
			submit.material = submitRed;
			submit.transform.GetChild(2).GetComponent<Light>().color = Color.red;
		}
		else
		{
			submitMode = true;
			submit.material = submitGreen;
			submit.transform.GetChild(2).GetComponent<Light>().color = Color.green;
		}
	}

	void LightLed(int nb)
	{
		if (nb == 0)
		{
			led0.material = ledColor;
			led0.transform.GetChild(0).GetComponent<Light>().color = Color.green;
		}
		if (nb == 1)
		{
			led1.material = ledColor;
			led1.transform.GetChild(0).GetComponent<Light>().color = Color.green;
		}
		if (nb == 2)
		{
			led2.material = ledColor;
			led2.transform.GetChild(0).GetComponent<Light>().color = Color.green;
		}
		if (nb == 3)
		{
			led3.material = ledColor;
			led3.transform.GetChild(0).GetComponent<Light>().color = Color.green;
		}
		if (nb == 4)
		{
			led4.material = ledColor;
			led4.transform.GetChild(0).GetComponent<Light>().color = Color.green;
		}
	}

	void LightLedValid(int nb)
	{
		if (nb == 1)
		{
			ledValid1.material = ledColor;
			ledValid1.transform.GetChild(0).GetComponent<Light>().color = Color.green;
		}
		if (nb == 2)
		{
			ledValid2.material = ledColor;
			ledValid2.transform.GetChild(0).GetComponent<Light>().color = Color.green;
		}
		if (nb == 3)
		{
			ledValid3.material = ledColor;
			ledValid3.transform.GetChild(0).GetComponent<Light>().color = Color.green;
		}
	}

	void TurnOffLeds()
	{
		led0.material = ledUnlit;
		led0.transform.GetChild(0).GetComponent<Light>().color = Color.black;
		led1.material = ledUnlit;
		led1.transform.GetChild(0).GetComponent<Light>().color = Color.black;
		led2.material = ledUnlit;
		led2.transform.GetChild(0).GetComponent<Light>().color = Color.black;
		led3.material = ledUnlit;
		led3.transform.GetChild(0).GetComponent<Light>().color = Color.black;
		led4.material = ledUnlit;
		led4.transform.GetChild(0).GetComponent<Light>().color = Color.black;
	}

	void Randomize()
	{

		valOrigine = new int[5, 5];
		stringOrigine = new String[5, 5];

		System.Random rand = new System.Random();

		for (int i = 0; i < 5; i++)
		{
			int nb1Line = 0;
			int nb0Line = 0;
			for (int j = 0; j < 5; j++)
			{

				int k = 0;
				for (int l = 0; l < moduleId; l++)
				{
					k = rand.Next(2);
				}

				if (nb1Line >= 3) k = 0;
				if (nb0Line >= 3) k = 1;

				if (k == 1)
				{
					nb1++;
					nb1Line++;
				}
				else
				{
					nb0++;
					nb0Line++;
				}

				valOrigine[i, j] = k;
				stringOrigine[i, j] = k.ToString();
			}
		}
	}

	void CreatePuzzle()
	{

		//Randomizer
		Randomize();
		Debug.LogFormat("[Binary Grid #{0}] Stage {1} : there are {2} ones and {3} zeroes.", moduleId, nbSolved + 1, nb1, nb0);
		//Keys & pattern
		KeyGenerator();
		//Debug.Log (key[0]+""+key[1]+""+key[2]+""+key[3]+""+key[4]+""+key[5]);
		Debug.LogFormat("[Binary Grid #{0}] The primary key for stage {1} is {2}{3}{4}{5}{6}{7}.", moduleId, nbSolved + 1, key[0], key[1], key[2], key[3], key[4], +key[5]);

		GetPattern();
		GetKey2();
		//Debug.Log (key2[0]+""+key2[1]+""+key2[2]+""+key2[3]+""+key2[4]+""+key2[5]);
		//Solution
		Debug.LogFormat("[Binary Grid #{0}] The secondary key for stage {1} is {2}{3}{4}{5}{6}{7}.", moduleId, nbSolved + 1, key2[0], key2[1], key2[2], key2[3], key2[4], +key2[5]);
		SolutionGenerator();
		//Debug.Log (solution [0] + "," + solution [1]);
		Debug.LogFormat("[Binary Grid #{0}] The solution for stage {1} is ({2},{3}).", moduleId, nbSolved + 1, solution[0] + 1, solution[1] + 1);
		StartCoroutine(SetModule(stringOrigine));
	}

	IEnumerator SetModule(String[,] values)
	{
		nb0 = 0;
		nb1 = 0;
		TurnOffLeds();

		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j <= i; j++)
			{
				if (j < 5 && i - j < 5)
				{
					yield return new WaitForSeconds(0.01f);
					SetValue(i - j, j, values[i - j, j]);
				}
			}
		}


	}

	void ResetModule()
	{
		String[,] voidMatrix = new String[5, 5];
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				voidMatrix[i, j] = "";
			}
		}
		StartCoroutine(SetModule(voidMatrix));
	}

	IEnumerator NewPuzzle()
	{
		ResetModule();
		yield return new WaitForSeconds(1f);
		CreatePuzzle();
	}

	string TwitchHelpMessage = "Use '!{0} <reset|check|submit> to reset/check the grid, or toggle submit. Use '!{0} a1 b2' to press those squares. Commands can be shortened to the first letter and chained using spaces.";

	IEnumerator ProcessTwitchCommand(string command)
	{
		var parts = command.ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

		if (parts.All(x => new[] { "reset", "submit", "check", "r", "s", "c" }.Contains(x) || ("abcde".Contains(x[0].ToString()) && "12345".Contains(x[1].ToString()))))
		{
			yield return null;

			for (int i = 0; i < parts.Length; i++)
			{
				var part = parts[i];

				if (part == "reset" || part == "r")
				{
					yield return "trycancel";
					ResetGrid();
				}
				else if (part == "submit" || part == "s")
				{
					yield return "trycancel";
					Submit();
				}
				else if (part == "check" || part == "c")
				{
					yield return "trycancel";
					Check();
				}
				else
				{
					yield return "trycancel";
					PressButtonGrid(part[1] - '1', part.ToUpperInvariant()[0] - 65); // Change each character into its number - 1.
				}
				yield return new WaitForSeconds(.1f);
			}
		}
	}
}
