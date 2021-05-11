import './App.css';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { BrowserRouter as Router, Switch, Route } from 'react-router-dom';
import DragDropComponent from './Components/Todo/DragDrogComponent';
import NavBarComponent  from './Components/Navbar/NavBarComponent';
import { TodoTypeComponent } from './Components/TodoType/TodoTypeComponent';

function App() {
  return (
    <Router>
      <NavBarComponent />
      <Switch>
        <Route path="/" exact>
          <DragDropComponent />
        </Route>
        <Route path="/todostype">
          <TodoTypeComponent/>
        </Route>
      </Switch>
      <ToastContainer
        position="bottom-right"
        autoClose={5000}
        hideProgressBar={false}
        newestOnTop={false}
        closeOnClick
        rtl={false}
        pauseOnFocusLoss
        draggable
        pauseOnHover
      />
    </Router>
  );
}

export default App;
