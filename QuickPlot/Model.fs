namespace QuickPlot

open QuickPlot.DataSources

type t = 
    { dataSources : DataSources.t 
      series      : ScatterLines.t }

