using UnityEngine;
using System.Collections;

public class Bits  {

	public Bits() : this( 0 ){}
    public Bits(int mask) { m_Value = mask; }
 
	public int value{ get{ return m_Value; } }
	private int m_Value;

    public void clear() {
        m_Value = 0;
    }
 
	//Add the mask to flags
	public int on( int mask ){
		return m_Value |= mask;
	}
 
	//Remove the mask from flags
	public int off( int mask ){
		return m_Value &= ~mask;
	}
 
	//Toggle the mask into flags
	public int toggle( int mask ){
		return m_Value ^= mask;
	}
 
	//Check if mask is on
	public bool has( int mask ){
		return ( m_Value & mask ) == mask;
	}
}
