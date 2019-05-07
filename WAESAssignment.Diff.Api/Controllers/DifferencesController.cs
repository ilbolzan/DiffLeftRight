﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WAESAssignment.Diff.Api.Interfaces.Repository;
using WAESAssignment.Diff.Api.Entity;
using WAESAssignment.Diff.Api.Models;
using WAESAssignment.Diff.Api.DTO;
using WAESAssignment.Diff.Api.Service;
using WAESAssignment.Diff.Api.Interfaces.Service;

namespace WAESAssignment.Diff.Api.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class DiffController : ControllerBase
    {
        private readonly IDifferenceLeftRepository _differenceLeftRepository;
        private readonly IDifferenceRightRepository _differenceRightRepository;
        private readonly IDifferenceService _differenceService;

        public DiffController(
            IDifferenceLeftRepository differenceLeftRepository,
            IDifferenceRightRepository differenceRightRepository,
            IDifferenceService differenceService)
        {
            _differenceLeftRepository = differenceLeftRepository;
            _differenceRightRepository = differenceRightRepository;
            _differenceService = differenceService;
        }

        /// <summary>
        /// Executes the comparisson between both sides
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<ResultComparisson> GetDifference(int id)
        {
            try
            {
                var comparissonResult = _differenceService.Compare(id);

                return comparissonResult;
            }
            catch (NonExistentComparissonException e)
            {
                return BadRequest(e);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        /// <summary>
        /// Retrieves the left valur for the Diff
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/left")]
        public async Task<ActionResult<DifferenceLeft>> GetDifferenceLeft(int id)
        {
            try
            {
                var difference = await _differenceLeftRepository.GetById(id);

                if (difference == null)
                {
                    return NotFound();
                }

                return difference;
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }

        }

        /// <summary>
        /// Retrieves the right valur for the Diff
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/right")]
        public async Task<ActionResult<DifferenceRight>> GetDifferenceRight(int id)
        {
            var difference = await _differenceRightRepository.GetById(id);

            if (difference == null)
            {
                return NotFound();
            }

            return difference;
        }

        /// <summary>
        /// Creates the left value for the Diff
        /// </summary>
        /// <param name="id"></param>
        /// <param name="base64Left"></param>
        /// <returns></returns>
        [HttpPost("{id}/left")]
        public ActionResult<Difference> PostLeft(int id, string base64Left)
        {
            var differenceLeft = new DifferenceLeft(id, base64Left);

            _differenceLeftRepository.Add(differenceLeft);

            return CreatedAtAction("GetDifferenceLeft", new { id = differenceLeft.Id }, differenceLeft);
        }

        /// <summary>
        /// Creates the right valur for the Diff
        /// </summary>
        /// <param name="differenceRight"></param>
        /// <returns></returns>
        [HttpPost("{id}/right")]
        public ActionResult<Difference> PostRight(DifferenceRight differenceRight)
        {
            _differenceRightRepository.Add(differenceRight);

            return CreatedAtAction("GetDifference", new { id = differenceRight.Id }, differenceRight);
        }
    }
}
