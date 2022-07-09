package com.example.comp259final;

import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;

public class BudgetSummaryFragment extends Fragment {


    //Input References
    private EditText earningsET;
    private EditText loanET;
    private TextView totalIncomeTV;

    private Button calculateExpenseBtn;

    private TextView totalExpenseTV;
    private TextView surplusShortfallTV;

    Budget budget;

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        return inflater.inflate(R.layout.budget_summary_fragment, container, false);
    }

    @Override
    public void onStart() {
        super.onStart();

        //Instantiating inputs from the budget summary fragment activity
        earningsET = (EditText) getView().findViewById(R.id.earningsEditText);
        loanET = (EditText) getView().findViewById(R.id.loansEditText);
        totalIncomeTV = (TextView) getView().findViewById(R.id.totalIncomeTextView);

        calculateExpenseBtn = (Button) getView().findViewById(R.id.calculateExpensesButton);

        totalExpenseTV = (TextView) getView().findViewById(R.id.budget_TotalExpensesTextView);
        surplusShortfallTV = (TextView) getView().findViewById(R.id.surplusShortfallTextView);

        //Instance of budget
        budget = new Budget();

        //TextWatchers for total income
        earningsET.addTextChangedListener(earningsTextWatcher);
        loanET.addTextChangedListener(loanTextWatcher);


        //Attach a click handler to the button
        calculateExpenseBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                //Send fragment to MainActivity
                sendFragment();
            }
        });


     //--------------------------------Received From ExpenseAmountsFragment via MainActivity------------------------------------//

        //Retrieve the data passed from ExpenseAmountFragment and extract the bundle in budget Object.
        Bundle arguments = getArguments();
        extractBundle(arguments);


    }

    //TextWatchers for Income
    private TextWatcher earningsTextWatcher = new TextWatcher() {

        @Override
        public void onTextChanged(CharSequence s, int start, int before, int count) {

            try{
                budget.setEarnings(Double.parseDouble(s.toString()));
            }catch (NumberFormatException e){
                budget.setEarnings(0);
            }
            //UpdateDisplay
            displayAllTotal();
        }

        @Override
        public void beforeTextChanged(CharSequence s, int start, int count, int after) {}

        @Override
        public void afterTextChanged(Editable s) {}
    };


    private TextWatcher loanTextWatcher = new TextWatcher() {

        @Override
        public void onTextChanged(CharSequence s, int start, int before, int count) {

            try{
                budget.setLoans(Double.parseDouble(s.toString()));
            }catch (NumberFormatException e){
                budget.setLoans(0);
            }
            //Update display
            displayAllTotal();
        }

        @Override
        public void beforeTextChanged(CharSequence s, int start, int count, int after) {}

        @Override
        public void afterTextChanged(Editable s) {}
    };


    private void displayAllTotal(){
        budget.calculateTotalIncome();
        budget.calculateSurplusShortfall();

        //Display the totalIncome in TextView
        totalIncomeTV.setText(String.format("%.02f", budget.getTotalIncome()));
        surplusShortfallTV.setText(String.format("%.02f", budget.getSurplusShortfall()));
        totalExpenseTV.setText(String.format("%.02f", budget.getTotalExpenses()));
    }

    public void sendFragment(){
        //Create a bundle and store data in it
        Bundle result = new Bundle();
        result.putDouble("earnings",budget.getEarnings());
        result.putDouble("loan",budget.getLoans());
        result.putBoolean("isAnnual",budget.isAnnual());
        result.putDouble("rent", budget.getRent());
        result.putDouble("utilities", budget.getUtilities());
        result.putDouble("internet", budget.getInternet());
        result.putDouble("phone", budget.getPhone());
        result.putDouble("tuition", budget.getTuition());
        result.putDouble("book", budget.getBooks());
        result.putDouble("otherFees", budget.getOtherFees());


        //Sending bundle to ExpenseAmountsFragment via main activity
        getParentFragmentManager().setFragmentResult("FromBudgetFragment", result);
    }

    public void extractBundle(Bundle arguments){
        if(arguments != null){
            budget.setEarnings(arguments.getDouble("earnings"));
            budget.setLoans(arguments.getDouble("loan"));
            budget.setAnnual(arguments.getBoolean("isAnnual"));
            budget.setRent(arguments.getDouble("rent"));
            budget.setUtilities(arguments.getDouble("utilities"));
            budget.setInternet(arguments.getDouble("internet"));
            budget.setPhone(arguments.getDouble("phone"));
            budget.setTuition(arguments.getDouble("tuition"));
            budget.setBooks(arguments.getDouble("book"));
            budget.setOtherFees(arguments.getDouble("otherFees"));

            budget.setTotalHousingCost(arguments.getDouble("TotalHousingCost"));
            budget.setTotalEducationCost(arguments.getDouble("TotalEducationalCost"));
            budget.setTotalExpenses(arguments.getDouble("TotalExpenses"));

            displayAllTotal();

        }
    }


}
