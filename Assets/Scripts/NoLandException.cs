using System.Collections;

public class NoLandException: System.ApplicationException{
	public NoLandException(){}
	public NoLandException(string message): base(message){
	}
}
