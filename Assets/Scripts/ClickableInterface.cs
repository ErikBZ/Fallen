using UnityEngine;

public interface ClickableInterface{
	void clicked();
	void unClicked();
	void highlight();
	void highlight(bool boolean);
	GameObject getGO();
	int getX();
	int getY();
	string getDescription();
	Unit getUnit();
	bool occupied();
	void setWalkable(bool newBool);
	bool getInfected();
}