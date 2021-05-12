import { Modal, Button, Card } from 'react-bootstrap';
import { toast } from 'react-toastify';
import PropTypes from 'prop-types';
import { formatDate } from '../../Utilities/dateTimeConvert';

const DeleteTodoModalComponent = ({item, setTodos, onHide, show,}) => {
  const torles = () => {
   
    const requestOptions = {
      method: 'DELETE',
      headers: {
        'Content-Type': 'application/json'
      },
    };
    fetch(`${process.env.REACT_APP_API_URL}/api/todo/${item.id}`, requestOptions)
      .then((response) => {
        if (!response.ok) {
          toast.error('Sikertelen törlés');
        } else {
          setTodos((prev) => {
            const elozo = { ...prev };
            const index = elozo[item.type.name.toLowerCase()]
              .items.findIndex((o) => o.id === item.id);
            if (index !== -1) elozo[item.type.name.toLowerCase()].items.splice(index, 1);
            return elozo;
          });
          toast.success('Törlés sikeres');
        }
      }).catch(() => {
        toast.error('Kommunikáció a szerverrel megszakadt!');
      });
    onHide();
  };

  return (
    <Modal
      onHide={onHide}
      size="md"
      aria-labelledby="contained-modal-title-vcenter"
      centered
      show={show}
    >
      <Modal.Header closeButton className="bg-egyedi color border-color">
        <Modal.Title id="contained-modal-title-vcenter">
          <h5 className="text-center color">Tevénykenység törlése</h5>
        </Modal.Title>
      </Modal.Header>
      <Modal.Body className="bg-egyedi color">
        <p className="text-danger font-weight-bold">
          Biztos Törölni akarod a következő elemet?
        </p>
        <Card text="light" className="p-0 mb-2 bg-egyedi border-egyedi ">
          <Card.Header className="p-2 pl-3 border-color">
            <span className="font-weight-bolder color">{item.name}</span>
          </Card.Header>
          <Card.Body className="p-0 pl-3 pt-2 ">
            <Card.Text>{item.details}</Card.Text>
            <Card.Subtitle className="" />
            <footer className="small mb-2 mt-3">
              {formatDate(item.deadline)}
            </footer>
          </Card.Body>
        </Card>
      </Modal.Body>
      <Modal.Footer className="bg-egyedi border-color">
        <Button variant="secondary" onClick={onHide}>Bezár</Button>
        <Button variant="danger" onClick={torles}>Törlés</Button>
      </Modal.Footer>
    </Modal>
  );
};

DeleteTodoModalComponent.propTypes = {
  setTodos: PropTypes.func.isRequired,
  show: PropTypes.bool.isRequired,
  onHide: PropTypes.func.isRequired,
  item: PropTypes.object.isRequired,

};
export default DeleteTodoModalComponent;
