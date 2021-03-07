#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators
{
	public class ppkLegCounter : Indicator
	{

		private Direction currentDirection = Direction.NULL;
		private double currentLegHigh = -1;
		private double currentLegLow = -1;
		private double previousLegHigh = -1;
		private double previousLegLow = -1;
		private long longLegCount = 1;
		private long shortLegCount = 1;
		private Pivot lastPivot = Pivot.NULL;
		private long longLabelId = 0;
		private long shortLabelId = 0;
		private SimpleFont font = new SimpleFont("ARIAL", 13);
		private bool outsideBar = false;
		private long lastOutsideBar = -1;
		private long lastHighPivot = -1;
		private long lastLowPivot = -1;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Leg counting indicator";
				Name										= "ppkLegCounter";
				Calculate									= Calculate.OnEachTick;
				IsOverlay									= false;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
			}
			else if (State == State.Configure)
			{
			}
		}

		protected override void OnBarUpdate()
		{
			// reset outside bar flag on next bar
			if (lastOutsideBar != CurrentBar)
            {
				outsideBar = false;
            }

			DetermineDirection();
			UpdateLegCount();

			if (State == State.Historical)
            {
				// update legs for historical
				UpdateLegsHistorical();
            }
			else if (State == State.Realtime)
            {
				// update legs for real time
				UpdateLegsRealtime();
            }
		}

		private void UpdateLegsRealtime()
        {
			if (currentDirection == Direction.NULL)
			{
				return;
			}

			if (currentDirection == Direction.LONG && Low[0] < Low[1] && lastPivot == Pivot.LOW && lastHighPivot != CurrentBar - 1 && !outsideBar)
            {
				currentDirection = Direction.SHORT;
				MarkHighPivot();
            }
			else if (currentDirection == Direction.SHORT && High[0] > High[1] && lastPivot == Pivot.HIGH && lastLowPivot != CurrentBar - 1 && !outsideBar)
			{
				currentDirection = Direction.LONG;
				MarkLowPivot();
			}
			// outside bar check
			outsideBar = High[0] > High[1] && Low[0] < Low[1];
			if (outsideBar)
			{
				lastOutsideBar = CurrentBar;
			}
		}

		private void UpdateLegsHistorical()
        {
			if (currentDirection == Direction.NULL)
            {
				return;
            }

			if (High[0] > High[1] && Low[0] < Low[1])
            {
				outsideBar = true;
				lastOutsideBar = CurrentBar;
				if (currentDirection == Direction.LONG)
				{
					// switch direction -> mark the pivot -> if closed above previous bar we traded back up so switch direction again and mark the bar
					currentDirection = Direction.SHORT;
					MarkHighPivot();
					if (Close[0] > Close[1])
					{
						UpdateLegCount();
						currentDirection = Direction.LONG;
						MarkLowPivot();
					}
				}
				else if (currentDirection == Direction.SHORT)
                {
					// switch direction -> mark the pivot -> if closed below previous bar we traded back up so switch direction again and mark the bar
					currentDirection = Direction.LONG;
					MarkLowPivot();
					if (Close[0] < Close[1])
                    {
						UpdateLegCount();
						currentDirection = Direction.SHORT;
						MarkHighPivot();
                    }
                }
            }				
			else if (currentDirection == Direction.LONG && Low[0] < Low[1] && lastPivot == Pivot.LOW && lastHighPivot != CurrentBar - 1)
            {
				currentDirection = Direction.SHORT;
				// mark pivot
				MarkHighPivot();
				UpdateLegCount(); // not really needed as of yet but w/e
            }
			else if (currentDirection == Direction.SHORT && High[0] > High[1] && lastPivot == Pivot.HIGH && lastLowPivot != CurrentBar - 1)
            {
				currentDirection = Direction.LONG;
				// mark pivot
				MarkLowPivot();
				UpdateLegCount();
            }
        }

		private void MarkHighPivot()
        {
			MarkBar(shortLegCount);
			lastHighPivot = CurrentBar - 1;
			lastPivot = Pivot.HIGH;
			previousLegHigh = currentLegHigh;
			shortLegCount++;
			currentLegHigh = -1;
        }

		private void MarkLowPivot()
        {
			MarkBar(longLegCount);
			lastLowPivot = CurrentBar - 1;
			lastPivot = Pivot.LOW;
			previousLegLow = currentLegLow;
			longLegCount++;
			currentLegLow = -1;
        }

		private void MarkBar(long count)
        {
			if (currentDirection == Direction.LONG)
            {
				Draw.Text(this, "longLeg" + longLabelId++, false, "" + count, 1, Low[1] - TickSize * 3, 0, Brushes.White, font, TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
            }
			else if (currentDirection == Direction.SHORT)
            {
				Draw.Text(this, "shortLeg" + shortLabelId++, false, "" + count, 1, High[1] + TickSize * 3, 0, Brushes.White, font, TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
			}
        }

		private void UpdateLegCount()
        {
			if (currentDirection == Direction.NULL)
            {
				return;
            }
			if (currentDirection == Direction.LONG)
            {
				if (currentLegHigh == -1)
					currentLegHigh = High[0];
				if (High[0] > currentLegHigh)
					currentLegHigh = High[0];
				if (currentLegHigh > previousLegHigh)
					shortLegCount = 1;
            }
			else if (currentDirection == Direction.SHORT)
            {
				if (currentLegLow == -1)
					currentLegLow = Low[0];
				if (Low[0] < currentLegLow)
					currentLegLow = Low[0];
				if (currentLegLow < previousLegLow)
					longLegCount = 1;
            }
        }

		private void DetermineDirection()
        {
			if (currentDirection != Direction.NULL)
            {
				return;
            }
			if (CurrentBar > 1)
            {
				if (High[0] > High[1])
                {
					currentDirection = Direction.LONG;
					lastPivot = Pivot.LOW;
                }
				else if (Low[0] < Low[1])
                {
					currentDirection = Direction.SHORT;
					lastPivot = Pivot.HIGH;
                }
            }
        }

		enum Direction
        {
			LONG,
			SHORT,
			NULL
        }

		enum Pivot
        {
			HIGH,
			LOW,
			NULL
        }
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private ppkLegCounter[] cacheppkLegCounter;
		public ppkLegCounter ppkLegCounter()
		{
			return ppkLegCounter(Input);
		}

		public ppkLegCounter ppkLegCounter(ISeries<double> input)
		{
			if (cacheppkLegCounter != null)
				for (int idx = 0; idx < cacheppkLegCounter.Length; idx++)
					if (cacheppkLegCounter[idx] != null &&  cacheppkLegCounter[idx].EqualsInput(input))
						return cacheppkLegCounter[idx];
			return CacheIndicator<ppkLegCounter>(new ppkLegCounter(), input, ref cacheppkLegCounter);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.ppkLegCounter ppkLegCounter()
		{
			return indicator.ppkLegCounter(Input);
		}

		public Indicators.ppkLegCounter ppkLegCounter(ISeries<double> input )
		{
			return indicator.ppkLegCounter(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.ppkLegCounter ppkLegCounter()
		{
			return indicator.ppkLegCounter(Input);
		}

		public Indicators.ppkLegCounter ppkLegCounter(ISeries<double> input )
		{
			return indicator.ppkLegCounter(input);
		}
	}
}

#endregion
