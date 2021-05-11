import { Draggable } from 'react-beautiful-dnd';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { Card } from 'react-bootstrap';
import PropTypes from 'prop-types';
import { formatDate } from '../../Utilities/dateTimeConvert';

const ItemComponent = ({ functions, item, index }) => (
  <Draggable key={item.id} index={index} draggableId={item.id.toString()}>
    {(provided) => (
      <Card
        ref={provided.innerRef}
        {...provided.draggableProps}
        {...provided.dragHandleProps}
        text="light"
        className="p-0 mb-1 mb-2 bg-egyedi border-egyedi "
      >
        <Card.Header className="p-2 pl-3 border-color">
          <span className="font-weight-bolder color">{item.name}</span>
          <span className="float-right ">
            <FontAwesomeIcon
              icon={faTimes}
              className="delete-fa"
              onClick={() => {
                functions.setModalDeleteShow(true);
                functions.setSelectedItem(item);
              }}
            />
          </span>

        </Card.Header>
        <Card.Body className="p-0 pl-3 pt-2 ">
          <Card.Text>{item.details}</Card.Text>
          <footer className="small mb-2 mt-3">
            {formatDate(item.deadline)}
          </footer>
        </Card.Body>
      </Card>
    )}
  </Draggable>
);
ItemComponent.propTypes = {
  functions: PropTypes.object.isRequired,
  item: PropTypes.object.isRequired,
  index: PropTypes.number.isRequired,

};

export default ItemComponent;
