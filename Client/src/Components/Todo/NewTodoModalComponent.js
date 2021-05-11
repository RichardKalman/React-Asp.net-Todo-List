import { Modal, Button, Form } from 'react-bootstrap';

import { toast } from 'react-toastify';
import PropTypes from 'prop-types';

const NewTodoModalComponent = ({
  setTodos, mezo, onHide, show,
}) => (
  <Modal
    onHide={onHide}
    size="md"
    aria-labelledby="contained-modal-title-vcenter"
    centered
    show={show}
  >
    <Modal.Header className="bg-egyedi color border-color" closeButton>
      <Modal.Title id="contained-modal-title-vcenter">
        <h5 className="text-center color">Új Tevénykenység</h5>
      </Modal.Title>
    </Modal.Header>
    <Modal.Body className="bg-egyedi color">
      <Form onSubmit={
          (e) => {
            e.preventDefault();
            const item = {
              name: e.target.elements.name.value,
              details: e.target.elements.details.value,
              mezo,
              deadline: e.target.elements.date.value,
            };
            const data = new FormData();
            data.append('data', JSON.stringify(item));

            const requestOptions = {
              method: 'POST',
              body: data,
            };

            fetch(`${process.env.REACT_APP_API_URL}/api/todo`, requestOptions)
              .then((response) => {
                if (!response.ok) {
                  toast.error('Sikertelen Hozzáadás!');
                  return undefined;
                }
                return response.json();
              })
              .then((datares) => {
                if (datares !== undefined) {
                  setTodos((prev) => {
                    prev = { ...prev };
                    prev[mezo].items.push(datares);
                    return prev;
                  });
                  toast.success('Sikeres hozzáadás!');
                }
              })
              .catch(() => {
                toast.error('Kommunikáció a szerverrel megszakadt!');
              });
            onHide();
          }
        }
      >
        <Form.Group controlId="name">
          <Form.Label>Név:</Form.Label>
          <Form.Control
            type="text"
            name="name"
            placeholder="Írd be a tevénykenység nevét"
            className="bg-egyedi color border-color"
          />
        </Form.Group>
        <Form.Group controlId="details">
          <Form.Label>Leírás</Form.Label>
          <Form.Control type="text" name="details" placeholder="Írd be a tevénykenység rövid leírását" className="bg-egyedi color border-color" />
        </Form.Group>
        <Form.Group controlId="date">
          <Form.Label>Határidő</Form.Label>
          <Form.Control type="date" name="date" className="bg-egyedi border-color color" />
        </Form.Group>
        <Button variant="primary" type="submit">
          Küldés
        </Button>

      </Form>
    </Modal.Body>
    <Modal.Footer className="bg-egyedi border-color">
      <Button variant="secondary" onClick={onHide}>Bezár</Button>
    </Modal.Footer>
  </Modal>
);

NewTodoModalComponent.propTypes = {
  setTodos: PropTypes.func.isRequired,
  show: PropTypes.bool.isRequired,
  onHide: PropTypes.func.isRequired,
  mezo: PropTypes.string.isRequired,

};

export default NewTodoModalComponent;
