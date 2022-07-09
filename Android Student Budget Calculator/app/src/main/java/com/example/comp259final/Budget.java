package com.example.comp259final;

public class Budget {
    //Variables
    double Earnings;
    double Loans;
    double TotalIncome;
    double TotalExpenses;
    double SurplusShortfall;

    boolean IsAnnual;
    double Rent;
    double Utilities;
    double Internet;
    double Phone;
    double TotalHousingCost;

    double Tuition;
    double Books;
    double OtherFees;
    double TotalEducationCost;

    //Constructors
    public Budget(){
        //Default Constructor
    }

    public Budget(double earnings, double loans){
        Earnings = earnings;
        Loans = loans;
    }

    public Budget(boolean isAnnual, double rent, double utilities, double internet, double phone, double tuition, double books, double otherFees){
        IsAnnual = isAnnual;
        Rent = rent;
        Utilities = utilities;
        Internet = internet;
        Phone = phone;
        Tuition = tuition;
        Books = books;
        OtherFees = otherFees;
    }

    //Getters and Setters
    public double getEarnings() {
        return Earnings;
    }

    public void setEarnings(double earnings) {
        Earnings = earnings;
    }

    public double getLoans() {
        return Loans;
    }

    public void setLoans(double loans) {
        Loans = loans;
    }

    public double getTotalIncome() {
        return TotalIncome;
    }

    public void setTotalIncome(double totalIncome) {
        TotalIncome = totalIncome;
    }

    public double getTotalExpenses() {
        return TotalExpenses;
    }

    public void setTotalExpenses(double totalExpenses) {
        TotalExpenses = totalExpenses;
    }

    public double getSurplusShortfall() {
        return SurplusShortfall;
    }

    public void setSurplusShortfall(double surplusShortfall) {
        SurplusShortfall = surplusShortfall;
    }

    public boolean isAnnual() {
        return IsAnnual;
    }

    public void setAnnual(boolean annual) {
        IsAnnual = annual;
    }

    public double getRent() {
        return Rent;
    }

    public void setRent(double rent) {
        Rent = rent;
    }

    public double getUtilities() {
        return Utilities;
    }

    public void setUtilities(double utilities) {
        Utilities = utilities;
    }

    public double getInternet() {
        return Internet;
    }

    public void setInternet(double internet) {
        Internet = internet;
    }

    public double getPhone() {
        return Phone;
    }

    public void setPhone(double phone) {
        Phone = phone;
    }

    public double getTotalHousingCost() {
        return TotalHousingCost;
    }

    public void setTotalHousingCost(double totalHousingCost) {
        TotalHousingCost = totalHousingCost;
    }

    public double getTuition() {
        return Tuition;
    }

    public void setTuition(double tuition) {
        Tuition = tuition;
    }

    public double getBooks() {
        return Books;
    }

    public void setBooks(double books) {
        Books = books;
    }

    public double getOtherFees() {
        return OtherFees;
    }

    public void setOtherFees(double otherFees) {
        OtherFees = otherFees;
    }

    public double getTotalEducationCost() {
        return TotalEducationCost;
    }

    public void setTotalEducationCost(double totalEducationCost) {
        TotalEducationCost = totalEducationCost;
    }

    //Calculates all totals
    public void calculateTotalHousingCost(){
        double totalHousingCost;

        totalHousingCost = Rent + Utilities + Internet + Phone;

        if(IsAnnual == true){
            totalHousingCost = totalHousingCost/12; //Divide it by 12 months
        }

        setTotalHousingCost(totalHousingCost);
    }

    public void calculateTotalEducationalCost(){
        double totalEducationCost;

        totalEducationCost = Tuition + Books + OtherFees;

        if(IsAnnual == true){
            totalEducationCost = totalEducationCost/12; //Divide by 12 if annual radio button is checked
        }

        setTotalEducationCost(totalEducationCost);
    }

    public void calculateTotalExpenses(){
        double totalExpenses;
        totalExpenses = TotalHousingCost + TotalEducationCost;
        setTotalExpenses(totalExpenses);
    }

    public void calculateSurplusShortfall(){
        double surplusShortfall;
        surplusShortfall = TotalIncome - TotalExpenses;
        setSurplusShortfall(surplusShortfall);
    }

    public void calculateTotalIncome(){
        double totalIncome;
        totalIncome = Earnings + Loans;
        setTotalIncome(totalIncome);
    }

}
