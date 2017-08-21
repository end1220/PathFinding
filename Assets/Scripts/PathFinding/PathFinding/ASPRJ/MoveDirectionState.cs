using System;

public class MoveDirectionState
{
    public Int3 adjDir;
    public bool applied;
    public Int3 curAdjDir;
    public Int3 curDir;
    public bool enabled;
    public Int3 firstAdjDir;
    public Int3 firstDir;
    public const int theshold = 0xf1c3c;

    public void BeginMove()
    {
        this.applied = false;
    }

    public void EndMove()
    {
        if (!this.applied)
        {
            this.enabled = false;
        }
    }

    public bool Equals(MoveDirectionState other)
    {
        return (((((this.enabled == other.enabled) && (this.applied == other.applied)) && ((this.adjDir == other.adjDir) && (this.curAdjDir == other.curAdjDir))) && ((this.curDir == other.curDir) && (this.firstAdjDir == other.firstAdjDir))) && (this.firstDir == other.firstDir));
    }

    public override bool Equals(object obj)
    {
        return (((obj != null) && (base.GetType() == obj.GetType())) && this.Equals((MoveDirectionState) obj));
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public void Reset()
    {
        this.enabled = false;
        this.applied = false;
        this.adjDir = Int3.zero;
        this.firstAdjDir = Int3.zero;
        this.curAdjDir = Int3.zero;
        this.firstDir = Int3.zero;
        this.curDir = Int3.zero;
    }

    public void SetNewDirection(ref Int3 dir)
    {
        bool flag = false;
        if (this.enabled)
        {
            if (Int3.Dot(ref this.firstDir, ref dir) < 0xf1c3c)
            {
                flag = true;
            }
        }
        else
        {
            flag = true;
        }
        if (flag)
        {
            this.adjDir = dir;
            this.firstAdjDir = Int3.zero;
            this.curAdjDir = Int3.zero;
            this.firstDir = Int3.zero;
            this.enabled = false;
        }
        this.curDir = dir;
    }
}

