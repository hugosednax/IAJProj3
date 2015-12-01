using System;
using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.TacticalAnalysis.DataStructures;
using RAIN.Navigation.NavMesh;
using RAIN.Navigation.Graph;

namespace Assets.Scripts.IAJ.Unity.TacticalAnalysis
{
    public class InfluenceMap
    {
        public uint NodesPerFlood { get; set; }
        private NavMeshPathGraph NavMeshGraph { get; set; }
        private List<IInfluenceUnit> Units { get; set; }
        private float InfluenceThreshold { get; set; }
        private IInfluenceFunction InfluenceFunction { get; set; }
        private IOpenLocationRecord Open { get; set; }
        public IClosedLocationRecord Closed { get; set; }
        public bool InProgress { get; set; }

        public InfluenceMap(NavMeshPathGraph navMesh, IOpenLocationRecord open, IClosedLocationRecord closed, IInfluenceFunction influenceFunction, float influenceThreshold)
        {
            this.NavMeshGraph = navMesh;
            this.Open = open;
            this.Closed = closed;
            this.InfluenceFunction = influenceFunction;
            this.InfluenceThreshold = influenceThreshold;
            this.NodesPerFlood = 100;
        }

        public void Initialize(List<IInfluenceUnit> units)
        {
            this.Open.Initialize();
            this.Closed.Initialize();
            this.Units = units;
            
            foreach (var unit in units)
            {
                //I need to do this because in Recast NavMesh graph, the edges of polygons are considered to be nodes and not the connections.
                //Theoretically the Quantize method should then return the appropriate edge, but instead it returns a polygon
                //Therefore, we need to create one explicit connection between the polygon and each edge of the corresponding polygon for the search algorithm to work
                ((NavMeshPoly)unit.Location).AddConnectedPoly(unit.Location.Position);

                var locationRecord = new LocationRecord
                {
                    Influence = unit.DirectInfluence,
                    StrongestInfluenceUnit = unit,
                    Location = unit.Location
                };

                Open.AddToOpen(locationRecord);
            }

            this.InProgress = true;
        }

        //this method should return true if it finished processing, and false if it still needs to continue
        public bool MapFloodDijkstra()
        {
            this.Open.Initialize();
            this.Closed.Initialize();
            var processedNodes = 0;
            foreach(var unit in Units){ //initialize the open list
                var locationRecord = new LocationRecord();
                locationRecord.Location = unit.Location;
                locationRecord.StrongestInfluenceUnit = unit;
                locationRecord.Influence = unit.DirectInfluence;
                Open.AddToOpen(locationRecord);
            }
            while(Open.CountOpen() > 0){
                var currentRecord =  Open.GetBestAndRemove();
                if (processedNodes > 30) {
                    this.InProgress = false;
                    return true; 
                }
                Closed.AddToClosed(currentRecord);
                processedNodes++;
                List<NavigationGraphNode> adjacentNodes = new List<NavigationGraphNode>();
                for(int i=0; i<currentRecord.Location.OutEdgeCount;i++){
                    NavigationGraphNode node = currentRecord.Location.EdgeOut(i).ToNode;
                    adjacentNodes.Add(node);
                }
                foreach (NavigationGraphNode adjacentNode in adjacentNodes)
                {
                    LocationRecord adjacent = new LocationRecord();
                    adjacent.Location = adjacentNode;
                    var influence = InfluenceFunction.DetermineInfluence(currentRecord.StrongestInfluenceUnit, adjacentNode.Position);
                    if(influence < InfluenceThreshold) continue;
                    var neighborRecord = Closed.SearchInClosed(adjacent);
                    if(neighborRecord != null){
                        if(neighborRecord.Influence >= influence){
                            continue;
                        }
                        else{ 
                            Closed.RemoveFromClosed(neighborRecord);
                            processedNodes--;
                        }
                    }
                    else{
                        neighborRecord = Open.SearchInOpen(adjacent);
                        if(neighborRecord != null){
                            if(neighborRecord.Influence < influence){
                                neighborRecord.StrongestInfluenceUnit = currentRecord.StrongestInfluenceUnit;
                                neighborRecord.Influence = influence;
                            }
                            continue;
                        }
                        else{ //we found a new record not in open or closed
                            neighborRecord = new LocationRecord();
                            neighborRecord.Location = adjacent.Location;
                        }
                    }
                    neighborRecord.StrongestInfluenceUnit = currentRecord.StrongestInfluenceUnit;
                    neighborRecord.Influence = influence;
                    Open.AddToOpen(neighborRecord);
                }
                processedNodes = 0;
            }
            this.InProgress = false;
            //this.CleanUp();
            return true;
        }


        public void CleanUp()
        {
            foreach (var unit in this.Units)
            {
                ((NavMeshPoly)unit.Location).RemoveConnectedPoly();
            }
        }
        
    }
}
