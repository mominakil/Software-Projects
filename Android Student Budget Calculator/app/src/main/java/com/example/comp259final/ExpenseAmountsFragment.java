package com.example.comp259final;

import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.RadioGroup;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;


public class ExpenseAmountsFragment extends Fragment {

    //Instance of Budget Class
    Budget budget;

    //Input References
    private RadioGroup annualMonthlyRG;
    private EditText rentET;
    private EditText utilitiesET;
    private EditText internetET;
    private EditText phoneET;
    private TextView totalHousingCostTV;

    private EditText tuitionET;
    private EditText booksET;
    private EditText otherFeesET;
    private TextView totalEducationCostTV;

    private TextView totalExpensesTV;
    private Button acceptBtn;


    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        return inflater.inflate(R.layout.expense_amounts_fragment, container, false);
    }


    @Override
    public void onStart() {
        super.onStart();

        //Instantiating the EditText, TextView and Button from expense_amounts_fragment
        annualMonthlyRG = (RadioGroup) getView().findViewById(R.id.AnnualMonthlyRadioGrp);
        rentET =  (EditText) getView().findViewById(R.id.rentEditText);
        utilitiesET = (EditText) getView().findViewById(R.id.utilitiesEditText);
        internetET = (EditText) getView().findViewById(R.id.internetEditText);
        phoneET = (EditText) getView().findViewById(R.id.phoneEditText);
        totalHousingCostTV = (TextView) getView().findViewById(R.id.totalHousingCostTextView);

        tuitionET = (EditText) getView().findViewById(R.id.tuitionEditText);
        booksET = (EditText) getView().findViewById(R.id.booksEditText);
        otherFeesET = (EditText) getView().findViewById(R.id.otherFeesEditText);
        totalEducationCostTV = (TextView) getView().findViewById(R.id.totalEducationCostsTextView);

        totalExpensesTV = (TextView) getView().findViewById(R.id.expenses_TotalExpensesTextView);
        acceptBtn = (Button) getView().findViewById(R.id.AcceptButton);

        //Instantiating budget variable
        budget = new Budget();

        //Getting Argument from bundle and extracting in budget Object
        Bundle arguments = getArguments();
        extractBundle(arguments);


        //Setting Text change listeners for Housing/Utilities Cost
        rentET.addTextChangedListener(rentTextWatcher);
        utilitiesET.addTextChangedListener(utilitiesTextWatcher);
        internetET.addTextChangedListener(internetTextWatcher);
        phoneET.addTextChangedListener(phoneTextWatcher);

        //Setting Text change listeners for Education Cost
        tuitionET.addTextChangedListener(tuitionTextWatcher);
        booksET.addTextChangedListener(booksTextWatcher);
        otherFeesET.addTextChangedListener(otherFeesTextWatcher);

        //Set click handler for Accept button
        acceptBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                //Send the fragment to MainActivity
                sendFragment();
            }
        });

    }


    public void checkAnnual(){
        //Getting the checked radio button and storing it to budget object.
        Integer radioId = annualMonthlyRG.getCheckedRadioButtonId();

        //Checking if which radio button is checked and assigning boolean to budget object
        if(radioId == R.id.annualRadioButton){
            budget.setAnnual(true);
        }else if(radioId == R.id.monthlyRadioButton){
            budget.setAnnual(false);
        }
    }

    public void sendFragment(){
        //Check the radio button state before sending it.
        checkAnnual();

        //Calculate the total again before sending it.
        calculateAllExpenses();

        //Create a Bundle and Store variable
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

        result.putDouble("TotalHousingCost", budget.getTotalHousingCost());
        result.putDouble("TotalEducationalCost", budget.getTotalEducationCost());
        result.putDouble("TotalExpenses", budget.getTotalExpenses());

        //Sending it to main activity fragment
        getParentFragmentManager().setFragmentResult("FromExpenseAmtFragment", result);
    }

    //TextWatchers for Housing/Utilities Cost
    private TextWatcher rentTextWatcher = new TextWatcher() {

        @Override
        public void onTextChanged(CharSequence s, int start, int before, int count) {

            try{
                budget.setRent(Double.parseDouble(s.toString()));
            }catch (NumberFormatException e){
                budget.setRent(0);
            }
            //Update Display
            displayAllTotals();
        }

        @Override
        public void beforeTextChanged(CharSequence s, int start, int count, int after) {}

        @Override
        public void afterTextChanged(Editable s) {}
    };

    private TextWatcher utilitiesTextWatcher = new TextWatcher() {

        @Override
        public void onTextChanged(CharSequence s, int start, int before, int count) {

            try{
                budget.setUtilities(Double.parseDouble(s.toString()));
            }catch (NumberFormatException e){
                budget.setUtilities(0);
            }
            //Update Display
            displayAllTotals();
        }

        @Override
        public void beforeTextChanged(CharSequence s, int start, int count, int after) {}

        @Override
        public void afterTextChanged(Editable s) {}
    };

    private TextWatcher internetTextWatcher = new TextWatcher() {

        @Override
        public void onTextChanged(CharSequence s, int start, int before, int count) {

            try{
                budget.setInternet(Double.parseDouble(s.toString()));
            }catch (NumberFormatException e){
                budget.setInternet(0);
            }
            //Update Display
            displayAllTotals();
        }

        @Override
        public void beforeTextChanged(CharSequence s, int start, int count, int after) {}

        @Override
        public void afterTextChanged(Editable s) {}
    };

    private TextWatcher phoneTextWatcher = new TextWatcher() {

        @Override
        public void onTextChanged(CharSequence s, int start, int before, int count) {

            try{
                budget.setPhone(Double.parseDouble(s.toString()));
            }catch (NumberFormatException e){
                budget.setPhone(0);
            }
            //Update Display
            displayAllTotals();
        }

        @Override
        public void beforeTextChanged(CharSequence s, int start, int count, int after) {}

        @Override
        public void afterTextChanged(Editable s) {}
    };

    //TextWatchers for Education cost
    private TextWatcher tuitionTextWatcher = new TextWatcher() {

        @Override
        public void onTextChanged(CharSequence s, int start, int before, int count) {

            try{
                budget.setTuition(Double.parseDouble(s.toString()));
            }catch (NumberFormatException e){
                budget.setTuition(0);
            }
            //Update Display
            displayAllTotals();
        }

        @Override
        public void beforeTextChanged(CharSequence s, int start, int count, int after) {}

        @Override
        public void afterTextChanged(Editable s) {}
    };

    private TextWatcher booksTextWatcher = new TextWatcher() {

        @Override
        public void onTextChanged(CharSequence s, int start, int before, int count) {

            try{
                budget.setBooks(Double.parseDouble(s.toString()));
            }catch (NumberFormatException e){
                budget.setBooks(0);
            }
            //Update Display
            displayAllTotals();
        }

        @Override
        public void beforeTextChanged(CharSequence s, int start, int count, int after) {}

        @Override
        public void afterTextChanged(Editable s) {}
    };

    private TextWatcher otherFeesTextWatcher = new TextWatcher() {

        @Override
        public void onTextChanged(CharSequence s, int start, int before, int count) {

            try{
                budget.setOtherFees(Double.parseDouble(s.toString()));
            }catch (NumberFormatException e){
                budget.setOtherFees(0);
            }
            //Update Display
            displayAllTotals();
        }

        @Override
        public void beforeTextChanged(CharSequence s, int start, int count, int after) {}

        @Override
        public void afterTextChanged(Editable s) {}
    };

    public void displayAllTotals(){
        //Check Radio Button Checked for annual or monthly
        checkAnnual();

        //Calculate all Totals
        calculateAllExpenses();

        //Check if the Annual is true, then multiply all Totals from budget object by 12 months to make it annual
        if(budget.isAnnual() == true){
            totalHousingCostTV.setText(String.format("%.02f",(budget.getTotalHousingCost()*12)));
            totalEducationCostTV.setText(String.format("%.02f",(budget.getTotalEducationCost()*12)));
            totalExpensesTV.setText(String.format(String.format("%.02f",(budget.getTotalExpenses()*12))));
        }else{
            //If Annual is set to false, which means monthly is checked, then display Totals amounts from budget object.
            totalHousingCostTV.setText(String.format("%.02f",budget.getTotalHousingCost()));
            totalEducationCostTV.setText(String.format("%.02f",budget.getTotalEducationCost()));
            totalExpensesTV.setText(String.format(String.format("%.02f",budget.getTotalExpenses())));
        }
    }

    public void calculateAllExpenses(){
        //Calculate all Totals
        budget.calculateTotalHousingCost();
        budget.calculateTotalEducationalCost();
        budget.calculateTotalExpenses();
    }

    //Extracts the bundle received
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

            //Check the Annual or Monthly radio button based on budget object isAnnual boolean
            if(budget.isAnnual() == true){
                annualMonthlyRG.check(R.id.annualRadioButton);
            }else{
                annualMonthlyRG.check(R.id.monthlyRadioButton);
            }

            rentET.setText(String.format("%.02f",budget.getRent()));
            utilitiesET.setText(String.format("%.02f",budget.getUtilities()));
            internetET.setText(String.format("%.02f",budget.getInternet()));
            phoneET.setText(String.format("%.02f",budget.getPhone()));
            tuitionET.setText(String.format("%.02f",budget.getTuition()));
            booksET.setText(String.format("%.02f",budget.getBooks()));
            otherFeesET.setText(String.format("%.02f",budget.getOtherFees()));

            displayAllTotals();

        }
    }

}
