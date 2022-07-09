package com.example.comp259final;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.fragment.app.FragmentManager;
import androidx.fragment.app.FragmentResultListener;
import androidx.fragment.app.FragmentTransaction;

import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;

public class MainActivity extends AppCompatActivity {

    //Global variable references
    Menu AppMenu;
    BudgetSummaryFragment budgetSummaryFragment;
    ExpenseAmountsFragment expenseAmountsFragment;
    String Expenses = "Expenses";
    String Accept = "Accept";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        //Create a fragment Manager instance
        FragmentManager fragmentManager = getSupportFragmentManager();

        //Using fragment manager to create / start fragment transaction
        FragmentTransaction fragmentTransaction = fragmentManager.beginTransaction();

        //Initializing an instance of BudgetSummaryFragment
        budgetSummaryFragment = new BudgetSummaryFragment();

        //budgetSummaryFragment.setAppMenu(AppMenu);

        fragmentTransaction.replace(R.id.fragment_container_view, budgetSummaryFragment, "budgetSummary");
        fragmentTransaction.commit();



      // ---------------------------Received From BudgetSummaryFragment-----Sending to ExpenseAmountsFragment---------------------------------------//

        //1. Set a listener to receive result from BudgetSummaryFragment (and then pass this data on to ExpenseAmountsFragment)
        fragmentManager.setFragmentResultListener("FromBudgetFragment", this, new FragmentResultListener() {
            @Override
            public void onFragmentResult(@NonNull String requestKey, @NonNull Bundle resultBundle) {


                //Initializing an instance of ExpenseAmountsFragment
                expenseAmountsFragment = new ExpenseAmountsFragment();

                //Set the argument of ExpenseAmountsFragment to add bundle received directly from BudgetSummaryActivity
                expenseAmountsFragment.setArguments(resultBundle);

                AppMenu.findItem(R.id.expenses_accept).setTitle(Accept);

                FragmentTransaction fragmentTransaction = fragmentManager.beginTransaction();
                fragmentTransaction.replace(R.id.fragment_container_view, expenseAmountsFragment, "ExpenseAmount");
                fragmentTransaction.commit();
            }
        });


      // ---------------------------Received From ExpenseAmountsFragment-----Sending to BudgetSummaryFragment--------------------------------------//

        //2. Set a Listener to receive results from ExpenseAmountFragments (and then pass this data on to BudgetSummaryFragment)
        fragmentManager.setFragmentResultListener("FromExpenseAmtFragment", this, new FragmentResultListener() {
            @Override
            public void onFragmentResult(@NonNull String requestKey, @NonNull Bundle resultBundle) {

                //Set the argument to budgetSummaryFragment object
                budgetSummaryFragment.setArguments(resultBundle);


                AppMenu.findItem(R.id.expenses_accept).setTitle(Expenses);

                FragmentTransaction fragmentTransaction = fragmentManager.beginTransaction();
                fragmentTransaction.replace(R.id.fragment_container_view, budgetSummaryFragment, "budgetSummary");
                fragmentTransaction.commit();
            }
        });
    }


    //Menu bar instantiation and inflating the menu bar to the view
    public boolean onCreateOptionsMenu(Menu menu){
        getMenuInflater().inflate(R.menu.menuoptions, menu);
        AppMenu = menu;
        return true;
    }

    public boolean onOptionsItemSelected(MenuItem item){

        int id = item.getItemId();

        if(id == R.id.expenses_accept){
            if (AppMenu.findItem(R.id.expenses_accept).getTitle().equals(Expenses)){
                budgetSummaryFragment.sendFragment();
            } else if(AppMenu.findItem(R.id.expenses_accept).getTitle().equals(Accept)){
                expenseAmountsFragment.sendFragment();
            }
            return true;
        }else if(id == R.id.quit){
            finish();
            return true;
        }
        return super.onOptionsItemSelected(item);
    }


}