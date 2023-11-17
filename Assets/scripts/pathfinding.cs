using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pathfinding
{
    public map Map;
    


    public class node
    {
        public bool check = false;
        public int Hvalue = 0;
        public int Gvalue = -1;
        public int Fvalue() { return Hvalue + Gvalue; }
        public Vector2 parent = new Vector2(-1, -1);
    }



    Dictionary<Vector2, node> nodes = new Dictionary<Vector2, node>();
    List<Vector2> waiting_list = new List<Vector2>();
    public Vector2[] bestpath = new Vector2[0];
    List<Vector2[]> possible_paths = new List<Vector2[]>();



    public void try_find_shortest_path(out Vector2[] path)
    {
        path = new Vector2[0];
        possible_paths.Clear();
        waiting_list.Add(Map.startpos);
        if (pathcheck8(Map.startpos))
        {
            bestpath = new Vector2[0];
            int bestpathgvalue = 9999999;
           // foreach (Vector2[] paths in possible_paths)
           // {
                Debug.Log("PATH FOUND!!!!!!");
                int i = 0;
             //     foreach (Vector2 pos in paths)
                //  {
                //     i += try_set_G_value(pos, new Vector2(-1, -1));
                 // }

                 //  if (i < bestpathgvalue) { bestpathgvalue = i; bestpath = paths; }
           // }
            bestpath = possible_paths[0];
            if (bestpath.Length > 0)
            {
                Debug.Log("BEST PATH FOUND!!!!!!");

                string S= "";
                foreach (Vector2 pos in bestpath)
                {
                    S += "->" + pos;
                }
                Debug.Log(S);
                path = bestpath;
            }
            else
            {
                Debug.Log("No path couuld be found are  end is un reachable!!!!!!");
            }
        }

    }
    public void populatenodes()
    {
        waiting_list.Clear();
        for (int Xi = 0; Xi < Map.size.x; Xi++)
        {
            for (int Yi = 0; Yi < Map.size.y; Yi++)
            {
                if (nodes.ContainsKey(new Vector2(Xi, Yi))) { nodes.Remove(new Vector2(Xi, Yi)); }
                nodes.Add(new Vector2(Xi, Yi), new node());
            }
        }

    }

    int get_H_value(Vector2 current_pos, Vector2 to_pos)
    {
        int manathan_value = 0;
        if (to_pos.x > current_pos.x) { manathan_value += (int)(to_pos.x - current_pos.x); }
        else if (to_pos.x < current_pos.x) { manathan_value += (int)(current_pos.x - to_pos.x); }

        if (to_pos.y > current_pos.y) { manathan_value += (int)(to_pos.y - current_pos.y); }
        else if (to_pos.y < current_pos.y) { manathan_value += (int)(current_pos.y - to_pos.y); }

        return manathan_value;
    }

    int try_set_G_value(Vector2 from_pos, Vector2 to_pos)
    {
        int walk_value = -1;
        // analising position  for  walktype value
        if (Map.mappositions.TryGetValue(to_pos, out map.tile topos_tile))
        {
            // will set walk value 
            switch (topos_tile.Type)
            {
                case map.tiletype.free:

                    switch (topos_tile.State)
                    {
                        case map.state.none:
                            walk_value = 10000;
                            break;
                        case map.state.flag:
                            walk_value = -1;
                            break;
                        case map.state.questionmark:
                            walk_value = -1;
                            break;
                    }
                    break;
                case map.tiletype.mine:
                    switch (topos_tile.State)
                    {
                        case map.state.none:
                            walk_value = 10000;
                            break;
                        case map.state.flag:
                            walk_value = -1;
                            break;
                        case map.state.questionmark:
                            walk_value = -1;
                            break;
                    }
                    break;
                case map.tiletype.obstacle:
                    walk_value = -1;
                    break;
                case map.tiletype.revealed:
                    walk_value = 10;
                    break;
                case map.tiletype.start:
                    walk_value = 10;
                    break;
                case map.tiletype.finish:
                    walk_value = 10;
                    break;
                case map.tiletype.tresaure:
                    walk_value = 10;
                    break;
                default:
                    break;
            }

            // walkable gate
            if (walk_value < 0) { return walk_value; }

            //if diagonal do * 1.4
          //  if (Mathf.Abs(from_pos.x - to_pos.x) == Mathf.Abs(from_pos.y - to_pos.y)) { walk_value = (int)Mathf.Ceil(walk_value * 1.4f); }


            //error below




            //get the node info for pos we are walking to
            if (nodes.TryGetValue(to_pos, out node to_pos_node))
            {
                // make sure  it has parent.
                if (Map.isInbound(to_pos_node.parent))
                {                 
                    //check if the current  pos G value is smaller than the parant's  g value.
                    if (nodes[from_pos].Gvalue < nodes[to_pos_node.parent].Gvalue)
                    {
                        Debug.Log(to_pos + " changed its parents from  " + to_pos_node.parent + " to " + from_pos) ;
                        Debug.Log(to_pos +" : "+  nodes[to_pos_node.parent].Gvalue + "   on  " + nodes[from_pos].Gvalue);
                        to_pos_node.parent = from_pos;
                    }
                }
                // auto assign current pos if to_pos node has no parent
                else
                { to_pos_node.parent = from_pos; }
                // once parent is assigned
                // get path G value of parent
                if (nodes.ContainsKey(to_pos_node.parent))
                {
                if (Mathf.Abs(to_pos_node.parent.x - to_pos.x) == Mathf.Abs(to_pos_node.parent.y - to_pos.y)) { walk_value = (int)Mathf.Ceil(walk_value * 1.4f); }
                    //make sure you dont add up negative accidently
                    if (nodes[to_pos_node.parent].Gvalue >= 0)
                    {                        
                        walk_value += nodes[to_pos_node.parent].Gvalue;
                    }
                }
                //assigning  the G value on to pos node
                to_pos_node.Gvalue = walk_value;

                return walk_value;
            }
        }
        return -1;
    }

    public bool pathcheck8(Vector2 pos)
    {
        if (waiting_list.Count < 1) { return true; }
       //Debug.Log("g: "+pos);
        waiting_list.Remove(pos);

        if (!Map.isInbound(pos)) { return pathcheck8(Next_post_to_check()); }
        if (nodes[pos].check) { return pathcheck8(Next_post_to_check()); }
        if (!Map.mappositions.TryGetValue(pos, out map.tile currenttile)) { return pathcheck8(Next_post_to_check()); }
        nodes[pos].check = true;
        // not  memory efficient, may have to change back to first path greed 
        if (currenttile.Type == map.tiletype.finish)
        {
            possible_paths.Add(path());

             return pathcheck8(Next_post_to_check());
        }
        if (currenttile.Type == map.tiletype.obstacle) { return pathcheck8(Next_post_to_check()); }
        if (currenttile.State == map.state.flag) { return pathcheck8(Next_post_to_check()); }
        if (currenttile.State == map.state.questionmark) { return pathcheck8(Next_post_to_check()); }

        List<Vector2> neighbor = new List<Vector2>();
        neighbor.Add(pos + new Vector2(0, 1));//N
        neighbor.Add(pos + new Vector2(1, 0));//E
        neighbor.Add(pos + new Vector2(0, -1));//S
        neighbor.Add(pos + new Vector2(-1, 0));//W
        neighbor.Add(pos + new Vector2(1, 1));//NE       
        neighbor.Add(pos + new Vector2(1, -1));//SE       
        neighbor.Add(pos + new Vector2(-1, -1));//SW       
        neighbor.Add(pos + new Vector2(-1, 1));//NW

        // close position// and populate waiting list
        foreach (Vector2 neigh in neighbor)
        {
            if (!Map.isInbound(neigh)) { continue; }
            if (!Map.mappositions.TryGetValue(neigh, out map.tile neightile)) { continue; }
            if (neightile.Type == map.tiletype.obstacle) { continue; }
            if (neightile.Type == map.tiletype.start) { continue; }
            if (neightile.State == map.state.questionmark) { continue; }
            if (neightile.State == map.state.flag) { continue; }


            try_set_G_value(pos, neigh);
           

            // node never visited
            nodes.TryGetValue(neigh, out node neighbn);
            if (!neighbn.check)
            {
                //this true if the path is walkable
                if (neighbn.Gvalue >= 0)
                {
                    // only set h value once
                    neighbn.Hvalue = get_H_value(neigh, Map.finishpos);
                    if (!waiting_list.Contains(neigh))
                    {
                        waiting_list.Add(neigh);
                    }
                }
            }
         //   pop_info(neigh);
        }
        if (pathcheck8(Next_post_to_check())) { return true; }
        else { return false; }
    }

    void pop_info(Vector2 pos)
    {
       // if (!nodes.ContainsKey(pos)) { return; }
      //  Debug.Log("[" + pos + "]  GValue : " + nodes[pos].Gvalue + " HValue : " + nodes[pos].Hvalue + " FValue : " + nodes[pos].Fvalue() + " parent position is : " + nodes[pos].parent + " map value : " + Map.mappositions[pos].Type.ToString() + " / " + Map.mappositions[pos].State.ToString()); ;
    }

    Vector2 Next_post_to_check()
    {
        Vector2 goodone = new Vector2(-1, -1);

        bool first = true;
        foreach (Vector2 pos in waiting_list)
        {

            if (first) { goodone = pos; first = false; }
            else
            {
                //if (nodes[pos].Gvalue< 1) { continue; }uselses
                if (nodes[goodone].Fvalue() > nodes[pos].Fvalue())
                {
                    goodone = pos;
                }
            }
        }
        return goodone;
    }

    public Vector2[] path()
    {

        List<Vector2> reversepath = new List<Vector2>();
        reversepath.Add(Map.finishpos);
        tery = 0;
        reversepath = finish_to_start_list(reversepath);
        Vector2[] path2 = new Vector2[reversepath.Count];
        int j = 0;
        for (int i = reversepath.Count - 1; i >= 0; i--)
        {
            path2[j] = reversepath[i];
            j++;
        }
        return path2;
    }

    int tery = 0;
    List<Vector2> finish_to_start_list(List<Vector2> reversepath)
    {
        tery++;
        if (tery > 500)// max should be  20* 20 400 tile which will never happen.
        {
            return reversepath;
        }
        reversepath.Add(nodes[reversepath[reversepath.Count - 1]].parent);
        if (Map.mappositions[reversepath[reversepath.Count - 1]].Type != map.tiletype.start)
        {
            return finish_to_start_list(reversepath);
        }
        else
        {
            return reversepath;
        }
    }
}
