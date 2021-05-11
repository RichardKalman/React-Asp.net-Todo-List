
import { Modal, Button, Card, ListGroup  } from 'react-bootstrap';
import { toast } from 'react-toastify';
import PropTypes from 'prop-types';

const DeleteTodoTypeComponent = ({getTodoTypes, selected, setTodoType, onHide, show}) => {

  
  const torles = () => {
    let item = {
      id: selected.id
    }   

    var data = new FormData();
    data.append("data", JSON.stringify(item));

    const requestOptions = {
      method: 'DELETE',
      body: data
    };
    fetch(process.env.REACT_APP_API_URL+'/api/todotype', requestOptions)
      .then(response => {
        if(!response.ok){
          toast.error('Sikertelen törlés');          
        }
        else{
          response = response.json()
          setTodoType(prev => {
            prev = { ...prev }
            delete prev[selected.order];
            let tempprevarray = Object.keys(prev).map((key)  =>  { return getTodoTypes[key] });//objektból arrayt csinálunk
            let index = 1;
            tempprevarray.forEach((tt) =>{
              tt.order = index++;
            })
            tempprevarray =  tempprevarray.reduce((aggregalt, tt) => ({ ...aggregalt, [tt.order]: tt }), {})
            prev = {...tempprevarray}
      
      
            return prev;
          })
          toast.success('Törlés sikeres');          
        }
      }).catch(function() {
        toast.error('Kommunikáció a szerverrel megszakadt!');        
      });
      onHide();
  }
  
  return (
    <Modal
      onHide={onHide}
      size="md"
      aria-labelledby="contained-modal-title-vcenter"
      centered
      show={show}      
    >
      <Modal.Header closeButton className="bg-egyedi color border-color">
        <Modal.Title id="contained-modal-title-vcenter" >
          <h5 className="text-center color">Tevénykenység tábla törlés</h5>
        </Modal.Title>
      </Modal.Header>
      <Modal.Body className="bg-egyedi color">
        <Card.Text >Törlődik az összes Todo is ({selected.todoSize}) </Card.Text >       
        <Card.Text className="text-danger font-weight-bold m-0">
          Biztos Törölni akarod a következő Tevénykenység táblát? 
        </Card.Text>
        <ListGroup >      
            <ListGroup.Item className="bg-dark color border-color">
              <span className="font-weight-bold">{selected.name}</span>
              <span>, Todok száma: {selected.todoSize}</span>
            </ListGroup.Item>             
        </ListGroup>                   
      </Modal.Body>
      <Modal.Footer className="bg-egyedi border-color">
        <Button variant="secondary" onClick={onHide}>Bezár</Button>
        <Button variant="danger" onClick={torles}>Törlés</Button>

      </Modal.Footer>
    </Modal>
  );
}

DeleteTodoTypeComponent.propTypes = {
  setTodoType: PropTypes.func.isRequired,
  show: PropTypes.bool.isRequired,
  onHide: PropTypes.func.isRequired,
  getTodoTypes: PropTypes.object.isRequired,
  selected: PropTypes.object.isRequired,

};
export default DeleteTodoTypeComponent;