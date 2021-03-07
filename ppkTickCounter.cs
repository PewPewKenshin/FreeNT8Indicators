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
	public class ppkTickCounter : Indicator
	{

		private SimpleFont font = new SimpleFont("ARIAL", 13);

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Tick counter next to forming bar";
				Name										= "ppkTickCounter";
				Calculate									= Calculate.OnEachTick;
				IsOverlay									= false;
				DisplayInDataBox							= false;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= false;
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
			UpdateTickCounter();
		}

		private void UpdateTickCounter()
        {
			if (BarsPeriod == null || BarsPeriodType.Tick != BarsPeriod.BarsPeriodType)
            {
				return;
            }
			int tickCount = BarsPeriod.Value - Bars.TickCount;
			Draw.Text(this, "tickCount", false, "" + tickCount, -3, Close[0], 0, Brushes.White, font, TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
        }
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private ppkTickCounter[] cacheppkTickCounter;
		public ppkTickCounter ppkTickCounter()
		{
			return ppkTickCounter(Input);
		}

		public ppkTickCounter ppkTickCounter(ISeries<double> input)
		{
			if (cacheppkTickCounter != null)
				for (int idx = 0; idx < cacheppkTickCounter.Length; idx++)
					if (cacheppkTickCounter[idx] != null &&  cacheppkTickCounter[idx].EqualsInput(input))
						return cacheppkTickCounter[idx];
			return CacheIndicator<ppkTickCounter>(new ppkTickCounter(), input, ref cacheppkTickCounter);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.ppkTickCounter ppkTickCounter()
		{
			return indicator.ppkTickCounter(Input);
		}

		public Indicators.ppkTickCounter ppkTickCounter(ISeries<double> input )
		{
			return indicator.ppkTickCounter(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.ppkTickCounter ppkTickCounter()
		{
			return indicator.ppkTickCounter(Input);
		}

		public Indicators.ppkTickCounter ppkTickCounter(ISeries<double> input )
		{
			return indicator.ppkTickCounter(input);
		}
	}
}

#endregion
