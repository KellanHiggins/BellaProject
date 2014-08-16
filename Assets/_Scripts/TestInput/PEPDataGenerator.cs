using System;
using UnityEngine;
using System.Collections;

public class PEPDataGenerator : MonoBehaviour
{
	// **** required parameters
	public int Breathing = 15; // Default is 15
	public int Repeat = 6; // Default is 6
	public int MinPressure = 10; // Default is 10
	public int MaxPressure = 20; // Default is 20
	public int MinInhale = 3; // Default is 3
	public int MaxInhale = 4; // each breath take // Default is 4
	public int MinExhale = 3; // Default is 3
	public int MaxExhale = 4; // each breath take // Default is 4
	public int MinRest = 20; // Default is 20
	public int MaxRest = 40; // 3-4 seconds // Default is 40

	public int Status = 0;
	public int BreathCount = 0;
	public int RepeatCount = 0;

	// **** internal use
	[SerializeField]
	private int MAXPREPARE = 0;
	[SerializeField]
	private int FAILRATE = 0; // 50% Fail rate

	private DateTime prepareTime, inhaleTime, exhaleTime, restTime;

	public PEPDataGenerator()
	{
		initData();
	}

	public void initData()
	{
		Status = 0;
		BreathCount = 0;
		RepeatCount = 0;
		prepareTime = DateTime.Now;
	}

	public string GetStatus()
	{
		return Status + "-" + (BreathCount + 1) + "-" + (RepeatCount + 1);
	}

	public float GetNextValue()
	{
		float pepData = 0;

		if (Status == 0)
		{
			long prepareSeconds = getTimeDiffSecond(prepareTime);
			if (prepareSeconds > MAXPREPARE)
			{
				Status = 1;
				BreathCount = 0;
				RepeatCount = 0;
				inhaleTime = DateTime.Now;
			}
			else
			{
				pepData = 0; //indicate ready
			}

		}
		else
		{
			if (RepeatCount < Repeat)
			{
				if (Status == 3)
				{
					// ---- rest
					long restSeconds = getTimeDiffSecond(restTime);
					if (restSeconds > getRandom(MinRest, MaxRest))
					{
						Status = 1;
						BreathCount = 0;
						inhaleTime = DateTime.Now;
					}
					else
					{
						pepData = getRandom(0, 2); // show little active to indicate
						// rest now
					}

				}
				else
				{
					if (BreathCount < Breathing)
					{
						if (Status == 1)
						{
							// ---- inhale
							long inhaleSeconds = getTimeDiffSecond(inhaleTime);
							if (inhaleSeconds > getRandom(MinInhale, MaxInhale))
							{
								Status = 2;
								exhaleTime = DateTime.Now;
							}
							else
							{
								pepData = 0;
							}
						}

						if (Status == 2)
						{
							// ---- exhale
							long holdSeconds = getTimeDiffSecond(exhaleTime);

							if (holdSeconds > getRandom(MinExhale, MaxExhale))
							{
								Status = 1;
								BreathCount = BreathCount + 1;
								inhaleTime = DateTime.Now;
								pepData = 0;

							}
							else
							{
								pepData = getBreathValue();
							}
						}

					}
					else
					{
						Status = 3;
						RepeatCount = RepeatCount + 1;
						// take a 1 - 2 minutes rest
						BreathCount = 0;
						restTime = DateTime.Now;
					}
				}
			}
			else
			{
				Status = 4;
				pepData = 0; // indicate off line 
				//initData();
			}
		}
		return pepData;
	}

	private long getTimeDiffSecond(DateTime orgTime)
	{
		DateTime currTime = System.DateTime.Now;
		return (long)(currTime - orgTime).TotalSeconds;
	}

	private float getBreathValue()
	{
		float breathValue = 0;
		if (getRandom(1, 100) > FAILRATE)
		{ // 95% meet 10 - 20 cm required
			breathValue = getRandom(MinPressure, MaxPressure) + getRandomFloat();
		}
		else
		{
			if (getRandom(1, 100) > 5)
			{ // 95% failed because lower than 10 cm
				breathValue = getRandom(0, MinPressure - 1);
			}
			else
			{ // 5% failed because higher than 20 cm
				breathValue = getRandom(MaxPressure + 1, 40);
			}
		}
		return breathValue;
	}

	private int getRandom(int Low, int High)
	{
		System.Random r = new System.Random();
		return r.Next(Low, High);
	}

	private float getRandomFloat()
	{
		System.Random r = new System.Random();
		return (float)r.NextDouble();
	}
}